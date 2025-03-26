// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Home page component
// Finle Name: home.component.ts

import { Component } from '@angular/core';
import { RouterModule } from '@angular/router'; // ✅ importuj RouterModule

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterModule],
  template: `
    <h2>Vítejte ve hře SupremeCourt - Alice in Borderland</h2>
    <p>
      Tato hra je inspirována japonským seriálem Alice in Borderland. 
      Hra je postavena na principu hry "SupremeCourt".
    </p>
    <p>
      Přihlašte se nebo zaregistrujte pro zahájení hry.
    </p>
  `,
})
export class HomePageComponent {}