import { Component, EventEmitter, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService, UserData } from '../../service/auth';

@Component({
  selector: 'app-topbar',
  imports: [RouterLink],
  templateUrl: './topbar.html',
  styleUrl: './topbar.css',
})
export class Topbar {
  @Output() toggleSidebar = new EventEmitter<void>();
  isMenuOpen = false;

  firstName: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit() {
    const user: UserData | null = this.authService.getUser();

    if (user && user.nomeCompleto) {
      this.firstName = user.nomeCompleto.split(' ')[0]; // pega só o primeiro nome
    }
  }

  logout() {
    this.authService.logout();
  }
}
