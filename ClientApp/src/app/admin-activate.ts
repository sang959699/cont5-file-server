import { CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from "@angular/router";
import { Observable } from "rxjs";
import { LoginService } from "./services/login.service";
import { Injectable } from "@angular/core";
import { take, map, first } from "rxjs/operators";

@Injectable()
export class AdminActivate implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean>|Promise<boolean>|boolean {
    return this.loginService.checkAdminRole().asObservable();
  }
}