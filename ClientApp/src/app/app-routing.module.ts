import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { FileListComponent } from './components/file-list/file-list.component';
import { VideoPlayerComponent } from './components/video-player/video-player.component';
import { LoginComponent } from './components/login/login.component';
import { LoginActivate } from './login-activate';
import { AdminActivate } from './admin-activate';
import { AuditViewerComponent } from './components/audit-viewer/audit-viewer.component';
import { AriaNgComponent } from './components/aria-ng/aria-ng.component';
import { BookmarkComponent } from './components/bookmark/bookmark.component';
import { TextEditorComponent } from './components/text-editor/text-editor.component';
import { StatsViewerComponent } from './components/stats-viewer/stats-viewer.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, pathMatch: 'full' },
  { path: 'fileList', component: FileListComponent, pathMatch: 'full', canActivate: [ LoginActivate ] },
  { path: 'bookmark', component: BookmarkComponent, pathMatch: 'full', canActivate: [LoginActivate] },
  { path: 'videoPlayer', component: VideoPlayerComponent, pathMatch: 'full', canActivate: [ LoginActivate ] },
  { path: 'auditViewer', component: AuditViewerComponent, pathMatch: 'full', canActivate: [ LoginActivate, AdminActivate ] },
  { path: 'ariaNg', component: AriaNgComponent, pathMatch: 'full', canActivate: [ LoginActivate, AdminActivate ] },
  { path: 'textEditor', component: TextEditorComponent, pathMatch: 'full', canActivate: [ LoginActivate, AdminActivate ] },
  { path: 'statsViewer', component: StatsViewerComponent, pathMatch: 'full', canActivate: [LoginActivate] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true, relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
