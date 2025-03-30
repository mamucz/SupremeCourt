// File: waiting-room.component.ts

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../Services/auth.service';

interface PlayerInfo {
  id: number;
  username: string;
}

@Component({
  selector: 'app-waiting-room',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './waiting-room.component.html',
  styleUrls: ['./waiting-room.component.scss']
})
export class WaitingRoomComponent implements OnInit {
  waitingRoomId!: number;
  players: PlayerInfo[] = [];
  playerId: number | null = null;
  countdown: number = 60;
  interval: any;
  message = '';
  error = '';

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.waitingRoomId = Number(this.route.snapshot.paramMap.get('id'));
    this.playerId = this.auth.getUserId();

    this.loadPlayers();
    this.startCountdown();

    // TODO: SignalR setup pro aktualizace
  }

  loadPlayers() {
    this.http.get<PlayerInfo[]>(`https://localhost:7078/api/waitingroom/${this.waitingRoomId}/players`, {
      headers: this.auth.getAuthHeaders()
    }).subscribe({
      next: (data) => this.players = data,
      error: () => this.error = 'Chyba při načítání hráčů.'
    });
  }

  startCountdown() {
    this.interval = setInterval(() => {
      this.countdown--;

      if (this.countdown <= 0) {
        clearInterval(this.interval);
      }
    }, 1000);
  }

  leaveRoom() {
    this.http.post(`https://localhost:7078/api/waitingroom/${this.waitingRoomId}/leave`, {
      playerId: this.playerId
    }, {
      headers: this.auth.getAuthHeaders()
    }).subscribe(() => {
      this.message = 'Opustil jsi místnost.';
      clearInterval(this.interval);
      // Redirect to list
    });
  }

  canStartGame(): boolean {
    return this.players.length === 5;
  }
}
