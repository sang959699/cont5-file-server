import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { LoadingBarRouterModule } from '@ngx-loading-bar/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { VgCoreModule } from '@videogular/ngx-videogular/core';
import { VgControlsModule } from '@videogular/ngx-videogular/controls';
import { VgOverlayPlayModule } from '@videogular/ngx-videogular/overlay-play';
import { VgBufferingModule } from '@videogular/ngx-videogular/buffering';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { LoginComponent } from './components/login/login.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FileListComponent } from './components/file-list/file-list.component';
import { VideoPlayerComponent } from './components/video-player/video-player.component';
import { BtnMoveDirective } from 'src/app/directives/btn-move.directive';
import { BtnDeleteDirective } from 'src/app/directives/btn-delete.directive';
import { BtnDeleteBookmarkDirective } from 'src/app/directives/btn-delete-bookmark.directive';
import { BtnSaveEditDirective } from 'src/app/directives/btn-save-edit.directive';
import { StopPropagationDirective } from 'src/app/directives/stop-propagation.directive';
import { LoginActivate } from './login-activate';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TxtNumericOnlyDirective } from './directives/txt-numeric-only.directive';
import { AuditViewerComponent } from './components/audit-viewer/audit-viewer.component';
import { AdminActivate } from './admin-activate';
import { AriaNgComponent } from './components/aria-ng/aria-ng.component';
import { BookmarkComponent } from './components/bookmark/bookmark.component';
import { TextEditorComponent } from './components/text-editor/text-editor.component';
import { StatsViewerComponent } from './components/stats-viewer/stats-viewer.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavMenuComponent,
    FileListComponent,
    VideoPlayerComponent,
    AuditViewerComponent,
    BtnMoveDirective,
    BtnDeleteDirective,
    BtnDeleteBookmarkDirective,
    BtnSaveEditDirective,
    StopPropagationDirective,
    TxtNumericOnlyDirective,
    AriaNgComponent,
    BookmarkComponent,
    TextEditorComponent,
    StatsViewerComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    LoadingBarHttpClientModule,
    LoadingBarRouterModule,
    NgSelectModule,
    BrowserAnimationsModule,
    NgbModule,
    VgCoreModule,
    VgControlsModule,
    VgOverlayPlayModule,
    VgBufferingModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: environment.production,
      // Register the ServiceWorker as soon as the application is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000'
    })
  ],
  providers: [LoginActivate, AdminActivate],
  bootstrap: [AppComponent]
})
export class AppModule { }
