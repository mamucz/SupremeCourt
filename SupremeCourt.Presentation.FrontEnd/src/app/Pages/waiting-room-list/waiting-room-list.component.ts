// Autor: Petr Ondra
// Description: Component for displaying waiting rooms with i18n
// File: waiting-room-list.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import * as signalR from '@microsoft/signalr';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { TimeFormatPipe } from '../../Pipes/time-format.pipe';

export interface PlayerDto {
  playerId: number;
  name: string;
}

export interface WaitingRoomDto {
  waitingRoomId: number;
  players: PlayerDto[];
  createdByPlayerId: number;
  createdByPlayerName: string;
  createdAt: string;
  playerCount: number;
  canStartGame: boolean;
  timeLeftSeconds: number;
}

@Component({
  selector: 'app-waiting-room-list',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule,TimeFormatPipe],
  templateUrl: './waiting-room-list.component.html',
  styleUrls: ['./waiting-room-list.component.scss']
})
export class WaitingRoomListComponent implements OnInit, OnDestroy {
  waitingRooms: WaitingRoomDto[] = [];
  message: string = '';
  error: string = '';
  private hubConnection!: signalR.HubConnection;

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private translate: TranslateService,
    private router: Router
  ) {
    const savedLang = localStorage.getItem('lang') || 'cs';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
  }

  ngOnInit(): void {
    this.loadWaitingRooms();
    this.setupSignalR();
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  loadWaitingRooms() {
    this.http.get<WaitingRoomDto[]>(`${environment.apiUrl}/waitingroom/waitingrooms`, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: (data) => {
        this.waitingRooms = data || [];
        this.error = '';
        if (this.waitingRooms.length === 0) {
          this.translate.get('WAITINGROOM.EMPTY').subscribe(text => this.message = text);
        }
      },
      error: (err) => {
        this.waitingRooms = [];
        this.message = '';
        this.translate.get('WAITINGROOM.CREATE_ERROR').subscribe(text => {
          this.error = err.error?.message || text;
        });
      }
    });
  }

  createWaitingRoom() {
    const playerId = this.auth.getUserId();
    if (!playerId) {
      this.translate.get('WAITINGROOM.CREATE_ERROR').subscribe(text => this.error = text);
      return;
    }

    this.http.post<any>(`${environment.apiUrl}/waitingroom/create`, { playerId }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: res => {
        this.translate.get('WAITINGROOM.CREATED').subscribe(text => {
          const id = res?.waitingRoomId ?? '?';
          this.message = `✅ ${text.replace('#', id)}`;
        });
        this.error = '';
      },
      error: err => {
        this.translate.get('WAITINGROOM.CREATE_ERROR').subscribe(text => {
          this.error = err.error?.message || text;
        });
        this.message = '';
      }
    });
  }

  joinRoom(gameId: number) {
    const playerId = this.auth.getUserId();
    if (!playerId) {
      this.error = 'Nejste přihlášen.';
      return;
    }

    this.http.post(`${environment.apiUrl}/waitingroom/join`, {
      waitingRoomId: gameId,
      playerId
    }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: () => {
        this.message = `✅ Připojeno k místnosti #${gameId}`;
        this.error = '';
        this.router.navigate([`/waiting-room/${gameId}`]);
      },
      error: err => {
        this.error = err.error?.message || 'Nepodařilo se připojit.';
        this.message = '';
      }
    });
  }

  private setupSignalR(): void {
    const hubUrl = `${environment.apiUrl.replace('/api', '')}/waitingRoomListHub`;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => this.auth.getToken() || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('✅ SignalR connected to WaitingRoomListHub');
        return this.hubConnection.invoke('JoinWaitingRoomList');
      })
      .catch(err => {
        console.error('❌ SignalR connection error:', err);
      });

    this.hubConnection.on('NewWaitingRoomCreated', (newRoom: WaitingRoomDto) => {
      this.waitingRooms.push(newRoom);
      this.translate.get('WAITINGROOM.CREATED').subscribe(text => {
        this.message = `🆕 ${text.replace('#', newRoom.waitingRoomId)}`;
      });
    });

    // ✅ Posloucháme event, který posílá backend (UpdateTimeLeft)
    this.hubConnection.on('UpdateTimeLeft', (data: { waitingRoomId: number, timeLeftSeconds: number }) => {
      const room = this.waitingRooms.find(r => r.waitingRoomId === data.waitingRoomId);
      if (room) {
        room.timeLeftSeconds = data.timeLeftSeconds;
      }
    });
  }
}
