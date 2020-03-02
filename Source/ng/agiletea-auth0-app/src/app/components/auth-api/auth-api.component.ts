import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/services/api.service';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-auth-api',
  templateUrl: './auth-api.component.html',
  styleUrls: ['./auth-api.component.sass']
})
export class AuthApiComponent implements OnInit {

  responseJson: string;

  constructor(private api: ApiService) { }

  ngOnInit() {
  }

  pingApi() {
    this.api.ping$().subscribe(
      res => this.responseJson = res
    );
  }
}
