import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from "./components/sidebar/sidebar";
import { Topbar } from "./components/topbar/topbar";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Sidebar, Topbar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('finops');

  sidebarVisible = false; // Controle global da sidebar (mobile)

  toggleSidebar() {
    this.sidebarVisible = !this.sidebarVisible;
  }
}
