// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Component for displaying waiting rooms
// File: waiting-room-list.component.ts

import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

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
export class WaitingRoomListComponent implements OnInit {
  waitingRooms: WaitingRoomDto[] = [];
  message: string = '';
  error: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadWaitingRooms();
  }

  loadWaitingRooms() {
    this.http.get<WaitingRoomDto[]>('https://localhost:7078/api/game/waitingrooms').subscribe({
      next: data => this.waitingRooms = data,
      error: err => this.error = 'Chyba při načítání čekacích místností.'
    });
  }

  joinRoom(gameId: number, playerId: number) {
    this.http.post('https://localhost:7078/api/game/join', {
      gameId: gameId,
      playerId: playerId
    }).subscribe({
      next: () => this.message = `✅ Připojeno k místnosti #${gameId}`,
      error: err => this.error = err.error?.message || 'Nepodařilo se připojit.'
    });
  }

  createGame() {
    this.http.post<any>('https://localhost:7078/api/game/create', {}).subscribe({
      next: res => {
        this.message = `✅ Vytvořena místnost #${res.waitingRoomId}`;
        this.loadWaitingRooms(); // obnovit seznam
      },
      error: err => this.error = 'Nepodařilo se vytvořit místnost.'
    });
  }
}
