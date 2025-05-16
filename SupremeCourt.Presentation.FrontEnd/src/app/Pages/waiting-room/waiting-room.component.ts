import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../Services/auth.service';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { TimeFormatPipe } from '../../Pipes/time-format.pipe';

interface PlayerDto {
  playerId: number;
  username: string;
  profileImageUrl?: string;
}

interface WaitingRoomDto {
  waitingRoomId: string;
  createdByPlayerId: number;
  createdAt: string;
  players: PlayerDto[];
  timeLeftSeconds: number;
  canStartGame: boolean;
}

@Component({
  selector: 'app-waiting-room',
  standalone: true,
  imports: [CommonModule, RouterModule, TimeFormatPipe],
  templateUrl: './waiting-room.component.html',
  styleUrls: ['./waiting-room.component.scss']
})
export class WaitingRoomComponent implements OnInit, OnDestroy {
  waitingRoomId!: string;
  waitingRoom: WaitingRoomDto | null = null;
  playerId: number | null = null;
  message = '';
  error = '';
  private hubConnection!: signalR.HubConnection;
  playerImages: { [playerId: number]: string } = {};

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.waitingRoomId = this.route.snapshot.paramMap.get('id')!;
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
        this.prepareImageUrls();
      },
      error: () => this.error = 'Chyba p�i na��t�n� m�stnosti.'
    });
  }

  setupSignalR() {
    const hubUrl = `${environment.apiUrl.replace('/api', '')}/waitingRoomHub`;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => {
          const token = this.auth.getToken() || '';
          console.log('??? JWT Token:', token);
          return token;
        }
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('? SignalR p�ipojeno k WaitingRoomHub:', this.waitingRoomId);
        return this.hubConnection.invoke('JoinRoom', this.waitingRoomId.toString());
      })
      .catch(err => {
        console.error('? Chyba p�i p�ipojen� k SignalR:', err);
      });

    this.hubConnection.on('WaitingRoomUpdated', (data: WaitingRoomDto) => {
      this.waitingRoom = data;
      this.prepareImageUrls();
    });

    this.hubConnection.on('CountdownTick', (secondsLeft: number) => {
      if (this.waitingRoom) {
        this.waitingRoom.timeLeftSeconds = secondsLeft;
      }
    });

    this.hubConnection.on('RoomExpired', () => {
      this.message = '? M�stnost byla zru�ena.';
      this.router.navigate(['/waiting-rooms']);
    });

    this.hubConnection.onclose(err => {
      console.warn('?? SignalR spojen� bylo ukon�eno:', err);
    });

    this.hubConnection.onreconnected(() => {
      console.log('?? SignalR znovu p�ipojeno, op�t vstupuji do m�stnosti:', this.waitingRoomId);
      this.hubConnection.invoke('JoinRoom', this.waitingRoomId.toString());
    });

    this.hubConnection.onreconnecting(err => {
      console.warn('?? SignalR znovu se p�ipojuje:', err);
    });
  }

  prepareImageUrls(): void {
    if (!this.waitingRoom) return;
    const now = Date.now();
    this.playerImages = {};
    for (const p of this.waitingRoom.players) {
      this.playerImages[p.playerId] = `${environment.signalRBaseUrl}/api/player/${p.playerId}/profile-picture?v=${now}`;
    }
  }

  leaveRoom() {
    this.http.post(`${environment.apiUrl}/waitingroom/${this.waitingRoomId}/leave/${this.playerId}`, {}, {
      headers: this.auth.getAuthHeaders()
    }).subscribe(() => {
      this.message = 'Opustil jsi m�stnost.';
      this.router.navigate(['/waiting-rooms']);
    });
  }

  canLeave(): boolean {
    return this.waitingRoom?.players?.some(p => p.playerId === this.playerId) ?? false;
  }

  canStartGame(): boolean {
    return !!this.waitingRoom &&
           this.waitingRoom.canStartGame &&
           this.playerId === this.waitingRoom.createdByPlayerId;
  }

  startGame(): void {
    console.log('?? Zah�jen� hry');
    // TODO: odeslat po�adavek na zah�jen� hry
  }

  getPlayerImageUrl(player: PlayerDto): string {
    return this.playerImages[player.playerId] || 'assets/img/default-avatar.png';
  }

  addAiPlayer(type: string): void {
    this.http.post(
      `${environment.apiUrl}/waitingroom/${this.waitingRoomId}/add-ai`,
      { type },
      { headers: this.auth.getAuthHeaders() }
    ).subscribe({
      next: () => this.message = `AI hr�� (${type}) p�id�n.`,
      error: () => this.error = 'Chyba p�i p�id�v�n� AI hr��e.'
    });
  }

  canAddAiPlayer(): boolean {
    return this.waitingRoom !== null &&
           this.playerId === this.waitingRoom.createdByPlayerId &&
           this.waitingRoom.players.length < 5;
  }

  getEmptyAiSlots(): number[] {
    if (!this.waitingRoom) return [];
    const slots = 5 - this.waitingRoom.players.length;
    return Array(slots).fill(0).map((_, i) => i);
  }

  onImageError(event: Event): void {
    (event.target as HTMLImageElement).src = 'assets/img/default-avatar.png';
  }
}
