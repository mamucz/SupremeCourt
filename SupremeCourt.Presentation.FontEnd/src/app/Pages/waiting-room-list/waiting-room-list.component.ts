// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Component for displaying waiting rooms
// File: waiting-room-list.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import * as signalR from '@microsoft/signalr';

interface WaitingRoomDto {
  waitingRoomId: number;
  createdAt: string;
  createdBy: string;
  playerCount: number;
}

@Component({
  selector: 'app-waiting-room-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
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
    private auth: AuthService
  ) {}

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
    this.http.get<WaitingRoomDto[]>('https://localhost:7078/api/waitingroom/waitingrooms', {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: (data) => {
        this.waitingRooms = data || [];
        this.error = '';
        if (this.waitingRooms.length === 0) {
          this.message = 'Žádné čekací místnosti zatím nejsou.';
        }
      },
      error: (err) => {
        this.waitingRooms = [];
        this.message = '';
        this.error = err.error?.message || 'Chyba při načítání čekacích místností.';
      }
    });
  }

  createWaitingRoom() {
    const playerId = this.auth.getUserId();
    if (!playerId) {
      this.error = 'Chybí ID hráče (playerId).';
      return;
    }

    this.http.post<any>('https://localhost:7078/api/waitingroom/create', { playerId }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: res => {
        this.message = `✅ Vytvořena místnost #${res.waitingRoomId}`;
        this.error = '';
        // Nepotřebujeme volat loadWaitingRooms(), SignalR se postará
      },
      error: err => {
        this.error = err.error?.message || 'Nepodařilo se vytvořit místnost.';
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

    this.http.post('https://localhost:7078/api/waitingroom/join', {
      gameId,
      playerId
    }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: () => {
        this.message = `✅ Připojeno k místnosti #${gameId}`;
        this.error = '';
      },
      error: err => {
        this.error = err.error?.message || 'Nepodařilo se připojit.';
        this.message = '';
      }
    });
  }

  private setupSignalR(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7078/waitingRoomHub', {
        accessTokenFactory: () => this.auth.getToken() || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('✅ SignalR připojeno');
        this.hubConnection.invoke('JoinWaitingRoomList'); // připojení ke skupině
      })
      .catch(err => {
        console.error('❌ Chyba při připojování SignalR:', err);
      });

    this.hubConnection.on('NewWaitingRoomCreated', (newRoom: WaitingRoomDto) => {
      this.waitingRooms.push(newRoom);
      this.message = `🆕 Nová místnost #${newRoom.waitingRoomId} byla vytvořena.`;
    });
  }
}
