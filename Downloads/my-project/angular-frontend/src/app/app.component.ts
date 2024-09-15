import { Component, OnInit } from '@angular/core';
import { DataService } from './services/data.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  tasks: string[] = [];
  newTask: string = '';
  editingTask: number | null = null;
  editingTaskContent: string = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.getAllTasks();
  }

  getAllTasks() {
    this.dataService.getTasks().subscribe(response => {
      this.tasks = response;
    });
  }

  addTask() {
    if (this.newTask.trim()) {
      this.dataService.addTask(this.newTask).subscribe(() => {
        this.getAllTasks();
        this.newTask = '';
      });
    }
  }

  editTask(index: number) {
    this.editingTask = index;
    this.editingTaskContent = this.tasks[index];
  }

  saveEditTask() {
    if (this.editingTask !== null) {
      this.dataService.editTask(this.editingTask, this.editingTaskContent).subscribe(() => {
        this.getAllTasks();
        this.editingTask = null;
        this.editingTaskContent = '';
      });
    }
  }

  deleteTask(index: number) {
    this.dataService.deleteTask(index).subscribe(() => {
      this.getAllTasks();
    });
  }
}
