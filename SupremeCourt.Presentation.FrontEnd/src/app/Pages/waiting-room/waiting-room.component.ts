import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../Services/auth.service';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

interface PlayerDto {
  playerId: number;
  username: string;
  profileImageUrl?: string;
}

interface WaitingRoomDto {
  waitingRoomId: number;
  createdByPlayerId: number;
  createdAt: string;
  players: PlayerDto[];
  timeLeftSeconds: number;
  canStartGame: boolean;
}

@Component({
  selector: 'app-waiting-room',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './waiting-room.component.html',
  styleUrls: ['./waiting-room.component.scss']
})
export class WaitingRoomComponent implements OnInit, OnDestroy {
  waitingRoomId!: number;
  waitingRoom: WaitingRoomDto | null = null;
  playerId: number | null = null;
  message = '';
  error = '';
  private hubConnection!: signalR.HubConnection;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.waitingRoomId = Number(this.route.snapshot.paramMap.get('id'));
    this.playerId = this.auth.getUserId();

    this.loadWaitingRoom();
    this.setupSignalR();
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  loadWaitingRoom() {
    this.http.get<WaitingRoomDto>(`${environment.apiUrl}/waitingroom/${this.waitingRoomId}`, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: (data) => {
        this.waitingRoom = data;
      },
      error: () => this.error = 'Chyba při načítání místnosti.'
    });
  }

  setupSignalR() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/waitingRoomHub`, {
        accessTokenFactory: () => this.auth.getToken() || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => {
        this.hubConnection.invoke('JoinRoom', this.waitingRoomId.toString());
        console.log('✅ Připojeno do SignalR místnosti', this.waitingRoomId);
      })
      .catch(err => console.error('❌ Chyba SignalR:', err));

    this.hubConnection.on('WaitingRoomUpdated', (data: WaitingRoomDto) => {
      this.waitingRoom = data;
    });

    this.hubConnection.on('CountdownTick', (secondsLeft: number) => {
      if (this.waitingRoom) {
        this.waitingRoom.timeLeftSeconds = secondsLeft;
      }
    });

    this.hubConnection.on('RoomExpired', () => {
      this.message = '⛔ Místnost byla zrušena.';
      this.router.navigate(['/waiting-rooms']);
    });
  }

  leaveRoom() {
    this.http.post(`${environment.apiUrl}/waitingroom/${this.waitingRoomId}/leave`, {
      playerId: this.playerId
    }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe(() => {
      this.message = 'Opustil jsi místnost.';
      this.router.navigate(['/waiting-rooms']);
    });
  }

  canLeave(): boolean {
    return this.waitingRoom?.players?.some(p => p.playerId === this.playerId) ?? false;
  }

  canStartGame(): boolean {
    if (!this.waitingRoom) return false;
    return this.waitingRoom.canStartGame && this.playerId === this.waitingRoom.createdByPlayerId;
  }

  startGame(): void {
    // TODO: Odeslat požadavek na zahájení hry (např. přes HTTP)
    console.log('🚀 Zahájení hry');
  }

  getPlayerImageUrl(player: PlayerDto): string {
    return `${environment.signalRBaseUrl}/api/player/${player.playerId}/profile-picture` + '?v=' + Date.now();
  }

  addAiPlayer(type: string): void {
    this.http.post(
      `${environment.apiUrl}/waitingroom/${this.waitingRoomId}/add-ai`,
      { type }, // tělo požadavku – např. "Random"
      { headers: this.auth.getAuthHeaders() }
    ).subscribe({
      next: () => this.message = `AI hráč (${type}) přidán.`,
      error: () => this.error = 'Chyba při přidávání AI hráče.'
    });
  }
  onImageError(event: Event): void {
  (event.target as HTMLImageElement).src = 'assets/img/default-avatar.png';
}

}
