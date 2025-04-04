// Autor: Petr Ondra
// Date: 9.3.2021
// Description: Application configuration
// File Name: app.config.ts

import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { provideTranslate } from './Core/translate.providers'; // ✅ vlastní provider (viz níže)

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    provideHttpClient(),
    provideTranslate(),
  ]
};
