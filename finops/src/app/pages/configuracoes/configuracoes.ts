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
  showSuccessAlert = false;

  settings: SiteSettings = {
    theme: 'auto',
    language: 'pt-BR',
    defaultCurrency: 'BRL',
    dateFormat: 'dd/MM/yyyy',
    timezone: 'America/Sao_Paulo',
    enableNotifications: true,
    enableTwoFactor: false,
  };

  currencies = [
    { value: 'BRL', label: 'Real Brasileiro (BRL)' },
    { value: 'USD', label: 'Dólar Americano (USD)' },
    { value: 'EUR', label: 'Euro (EUR)' },
    { value: 'GBP', label: 'Libra Esterlina (GBP)' },
    { value: 'JPY', label: 'Iene Japonês (JPY)' },
  ];

  languages = [
    { value: 'pt-BR', label: 'Português (Brasil)' },
    { value: 'en-US', label: 'English (US)' },
    { value: 'es-ES', label: 'Español (España)' },
  ];

  dateFormats = [
    { value: 'dd/MM/yyyy', label: 'DD/MM/AAAA (31/12/2025)' },
    { value: 'MM/dd/yyyy', label: 'MM/DD/AAAA (12/31/2025)' },
    { value: 'yyyy-MM-dd', label: 'AAAA-MM-DD (2025-12-31)' },
  ];

  timezones = [
    { value: 'America/Sao_Paulo', label: 'São Paulo (UTC-3)' },
    { value: 'America/Rio_Branco', label: 'Rio Branco (UTC-5)' },
    { value: 'America/Fortaleza', label: 'Fortaleza (UTC-3)' },
    { value: 'America/Manaus', label: 'Manaus (UTC-4)' },
    { value: 'America/Bahia', label: 'Bahia (UTC-3)' },
    { value: 'UTC', label: 'UTC (Universal)' },
  ];

  saveSettings() {
    localStorage.setItem('site-settings', JSON.stringify(this.settings));

    this.savedMessage = 'Configurações salvas com sucesso!';
    this.showSuccessAlert = true;

    setTimeout(() => {
      this.savedMessage = '';
      this.showSuccessAlert = false;
    }, 3000);
  }

  ngOnInit() {
    const saved = localStorage.getItem('site-settings');
    if (saved) {
      this.settings = JSON.parse(saved);
    }
  }
}
