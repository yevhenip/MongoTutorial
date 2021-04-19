import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AdminPanelComponent} from './admin-panel/admin-panel.component';
import {HomeComponent} from './home/home.component';
import {LoginComponent} from './login/login.component';
import {RegisterComponent} from './register/register.component';
import {AdminAuthGuard} from './services/auth/admin-auth-guard.service';
import {AnonymousAuthGuard} from './services/auth/anonymous-auth-guard.service';
import {AuthGuard} from './services/auth/auth-guard.service';

const routes: Routes = [
  {path: '', component: HomeComponent, canActivate: [AuthGuard]},
  {path: 'login', component: LoginComponent, canActivate: [AnonymousAuthGuard]},
  {path: 'register', component: RegisterComponent, canActivate: [AnonymousAuthGuard]},
  {path: 'admin', component: AdminPanelComponent, canActivate: [AdminAuthGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [AuthGuard, AdminAuthGuard, AnonymousAuthGuard]
})
export class AppRoutingModule {
}
