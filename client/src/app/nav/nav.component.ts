import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'nav-component',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any = {};

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
 
  }

  login(){
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      console.log("inciaste session");
    }, error =>{
      console.log(error);
    });
  }


  logout(){
    this.accountService.logout();
  }

  

}
