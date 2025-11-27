import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface SiteSettings {
  theme: string;
  language: string;
  defaultCurrency: string;
  dateFormat: string;
  timezone: string;
  enableNotifications: boolean;
  enableTwoFactor: boolean;
}

@Component({
  selector: 'app-configuracoes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './configuracoes.html',
  styleUrl: './configuracoes.css',
})
export class Configuracoes {
  
  savedMessage = '';

  settings: SiteSettings = {
    theme: 'auto',
    language: 'pt-BR',
    defaultCurrency: 'BRL',
    dateFormat: 'dd/MM/yyyy',
    timezone: 'America/Sao_Paulo',
    enableNotifications: true,
    enableTwoFactor: false
  };

  currencies = ['BRL', 'USD', 'EUR', 'GBP', 'JPY'];
  languages = ['pt-BR', 'en-US', 'es-ES'];
  dateFormats = ['dd/MM/yyyy', 'MM/dd/yyyy', 'yyyy-MM-dd'];
  timezones = [
    'America/Sao_Paulo',
    'America/Rio_Branco',
    'America/Fortaleza',
    'America/Manaus',
    'America/Bahia',
    'UTC'
  ];

  saveSettings() {
    localStorage.setItem('site-settings', JSON.stringify(this.settings));

    this.savedMessage = 'Configurações salvas com sucesso!';

    setTimeout(() => (this.savedMessage = ''), 2500);
  }

  ngOnInit() {
    const saved = localStorage.getItem('site-settings');
    if (saved) {
      this.settings = JSON.parse(saved);
    }
  }
}
