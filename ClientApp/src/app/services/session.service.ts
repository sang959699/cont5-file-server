import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SessionService {

  // public cont5Token: string;
  // public cont5Path: string;

  set cont5Token(value: string) {
    sessionStorage.setItem('Cont5.Token', value);
  }

  get cont5Token(): string {
    return sessionStorage.getItem('Cont5.Token') ?? "";
  }

  set cont5Path(value: string) {
    sessionStorage.setItem('Cont5.Path', value);
  }

  get cont5Path(): string {
    return sessionStorage.getItem('Cont5.Path') ?? "";
  }

  clear() {
    sessionStorage.clear();
  }
}