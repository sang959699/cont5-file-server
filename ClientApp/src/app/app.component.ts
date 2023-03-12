import { Component, HostListener } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { LoginService } from './services/login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent {
  constructor(private loginService: LoginService, private router: Router) { }

  isLoggedIn: boolean;

  ngOnInit() {
    this.loginService.isTokenValidateSubject.subscribe(s => this.isLoggedIn = s);
  }

  // @HostListener("window:beforeunload", ["$event"]) unloadHandler(event: Event) {
  //   let result = confirm("Changes you made may not be saved.");
  //   event.returnValue = false;
  // }
}
