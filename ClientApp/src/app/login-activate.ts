import { CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from "@angular/router";
import { Observable } from "rxjs";
import { LoginService } from "./services/login.service";
import { Injectable } from "@angular/core";
import { take, map } from "rxjs/operators";

@Injectable()
export class LoginActivate implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean>|Promise<boolean>|boolean {
    return this.loginService.validateToken().asObservable();
    // this.loginService.isLoggedIn();
    // if (sessionStorage.getItem('Cont5.Token') == null) {
    //     this.router.navigate(['/login']);
    // }
    // return true;
  }
}