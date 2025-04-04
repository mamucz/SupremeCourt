// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Home page component
// File Name: home.component.ts

import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterModule, TranslateModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomePageComponent {
  constructor(private translate: TranslateService) {
    this.translate.setDefaultLang('cs'); // výchozí jazyk
    this.translate.use('cs');            // použít výchozí
  }

  
}
