import {HttpClientModule} from "@angular/common/http";
import {NgModule} from "@angular/core";
import {ReactiveFormsModule} from "@angular/forms";
import {ErrorStateMatcher, ShowOnDirtyErrorStateMatcher} from "@angular/material/core";
import {BrowserModule} from "@angular/platform-browser";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {JwtModule} from "@auth0/angular-jwt";
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {AdminPanelComponent} from "./admin-panel/admin-panel.component";
import {AppRoutingModule} from "./app-routing.module";
import {AppComponent} from "./app.component";
import {CustomerFormComponent} from "./customer-form/customer-form.component";
import {CustomerPanelComponent} from "./customer-panel/customer-panel.component";
import {HomeComponent} from "./home/home.component";
import {LoginComponent} from "./login/login.component";
import {ManufacturerFormComponent} from "./manufacturer-form/manufacturer-form.component";
import {ManufacturerPanelComponent} from "./manufacturer-panel/manufacturer-panel.component";
import {MdComponentsModule} from "./md-components.module";
import {NavbarComponent} from "./navbar/navbar.component";
import {ProductFormComponent} from "./product-form/product-form.component";
import {RegisterComponent} from "./register/register.component";
import {AuthService} from "./services/auth/auth.service";
import {DataService} from "./services/data/data.service";
import {UserFormComponent} from "./user-form/user-form.component";
import {UserPanelComponent} from "./user-panel/user-panel.component";

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    ProductFormComponent,
    AdminPanelComponent,
    ManufacturerPanelComponent,
    UserPanelComponent,
    CustomerPanelComponent,
    ManufacturerFormComponent,
    UserFormComponent,
    CustomerFormComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    NgbModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: () => localStorage.getItem("jwtToken")
      }
    }),
    MdComponentsModule,
    ReactiveFormsModule
  ],
  providers: [
    AuthService,
    {provide: ErrorStateMatcher, useClass: ShowOnDirtyErrorStateMatcher}
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
