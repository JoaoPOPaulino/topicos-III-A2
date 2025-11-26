import { Component, EventEmitter, Output } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-topbar',
  imports: [RouterLink],
  templateUrl: './topbar.html',
  styleUrl: './topbar.css',
})
export class Topbar {
  @Output() toggleSidebar = new EventEmitter<void>(); // Avisa o pai para abrir/fechar sidebar
  isMenuOpen = false; // Controla o dropdown
}

