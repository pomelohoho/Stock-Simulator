import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';  // Import HttpClientModule
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DataService } from './services/data.service';  // Import DataService
import { FormsModule } from '@angular/forms';  // Import FormsModule


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FormsModule, 
    AppRoutingModule,
    HttpClientModule  

  ],
  providers: [DataService],  // Ensure DataService is provided
  bootstrap: [AppComponent]
})
export class AppModule { }
