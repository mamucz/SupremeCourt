# **Create project**:

```
ng new [project name]
```

*Select scss (ne server site rendering)*

## Project structure

- All project file by my is in src dir
- All is component (blocks)
- Very first file is index.html
- Not using java script but type script
- main file for TS is main.ts
- style.scss is file for css acros the project
- dir assets is for assets
- app include all code for pages. Here is place for my code

## Now we are go start default project as was created:

```
npm run start
```

- npm is packages services

- run is command

- start is sections from packege.json i root of project

- Starting targets is http://localhost:4200/
  I'm using angular 17 and text for searching examples is "I'm using Angular stand alone"

- The very first component watch is rendering first after runnig is app.component. Because is setted in root file main.ts
  and each component has 4 files

  

  **app.component has 4 files (all files is only one component):**

 - html is for html
 - sccs is cascade file
 This two file is defined in app.component.ts. The standard is the same name.

**app.component.ts:**

```react
import { Component } from '@angular/core'; // this is for import classes (c# using)
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root', //it is the name of class for html
  standalone: true, //don't change it
  imports: [RouterOutlet], //import all classes whitch are used in component
  templateUrl: './app.component.html', // path for target html file of componet
  styleUrl: './app.component.scss' // path for scss file of component
})
export class AppComponent { //it is the name of class and make the class as public
  title = 'SupremeCourterStudy'; //it's variable of class
}
```



- From app.component.html delete excluse last row  <router-outlet /> maps of components and is seted by file app.routes.ts in src dir
- In src/app create two folders Components,Pages and Services
- Set of Compact Folders off in VS Code
- The command for creating component: ng generate component [dir for new component] (Pages/AuthRegistr) - Pages is target folder and AuthRegistr is the name of new commponent

Now we are swetting router (app.route.ts)

```react
import { Routes } from '@angular/router';
import { AuthRegistrPagesComponent } from './Pages/auth-registr/auth-registr.component';

export const routes: Routes = [{path: 'Register', component: AuthRegistrPagesComponent}];
```

And now http://localhost:4200/Register are working!!!

Now edit auth-registr.component.html abd add two inputs:

<p>auth-registr works!</p>
<input type="text" placeholder="Name" [(ngModel)]="name">
<input type="password" placeholder="Password" [(ngModel)]="password">

ngModel is most easy way to take value from input. The name of is name and password

Chanege auth-registr.component.html

<p>auth-registr works!</p>
```react
<input type="text" placeholder="Name" [(ngModel)]="name">
<input type="password" placeholder="Password" [(ngModel)]="password">
<button (click)="OnSubmit()">Registr</button>

Change auth-registr.component.ts

import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-auth-registr',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './auth-registr.component.html',
  styleUrl: './auth-registr.component.scss'
})
export class AuthRegistrPagesComponent {
name: string = '';
password: string = '';
OnSubmit() {}
}
```

Now we are creating service:
run command: ng generate service Services/auth 
(auth.service.specs.ts is file for tests)











