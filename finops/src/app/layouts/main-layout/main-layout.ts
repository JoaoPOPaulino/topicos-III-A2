import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from '../../components/sidebar/sidebar';
import { Topbar } from '../../components/topbar/topbar';


@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, Sidebar, Topbar],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {
    protected readonly title = signal('finops');

    sidebarVisible = false; // Controle global da sidebar (mobile)

    toggleSidebar() {
      this.sidebarVisible = !this.sidebarVisible;
    }
}
