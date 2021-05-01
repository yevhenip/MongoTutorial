import {ComponentType} from "@angular/cdk/portal";
import {Component, ViewChild} from "@angular/core";
import {MatDialog} from "@angular/material/dialog";
import {MatPaginator} from "@angular/material/paginator";
import {MatTableDataSource} from "@angular/material/table";
import { GroupDataComponent } from "../group-data/group-data.component";
import {Log} from "../models/log";
import {DataService} from "../services/data/data.service";
import {LogService} from "../services/data/log/log.service";

@Component({template: ''})
export abstract class DataPanel {
  dataSource!: MatTableDataSource<Object>;
  resultsLength!: number;
  displayedColumns!: string[]
  logs!: Log[]

  constructor(private service: DataService, protected logService: LogService) {
  }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  async ngOnInit() {
    let response = await this.getAll();
    let data = response as [];

    this.resultsLength = data.length;
    this.dataSource = new MatTableDataSource<Object>(data);
    this.dataSource.paginator = this.paginator;
    console.log(await this.service.getPage(1, 5));
    this.logs = (await this.logService.getActual() as Log[]).reverse();
    for (let log of this.logs) {
      log.serializedData = JSON.parse(log.serializedData);
      log.actionDate = new Date(log.actionDate).toUTCString();
      log.actionDate = log.actionDate.slice(0, log.actionDate.length - 4);
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    this.dataSource.paginator?.firstPage();
  }

  getAll() {
    return this.service.getAll();
  }

  openDialogForCreation(dialogItem: MatDialog, component: ComponentType<any>, type: string) {
    let dialog = dialogItem.open(component);
    dialog.afterClosed().subscribe(async result => {
      if (result) {
        let creationResult = await this.service.create(result);
        this.dataSource.data.push(creationResult);
        this.dataSource._updateChangeSubscription();
        this.dataSource.paginator?.lastPage();
        this.addLog('created ' + type, creationResult);
      }
    });
  }

  openDialogForEditing(item: any, dialogItem: MatDialog, component: ComponentType<any>, type: any) {
    let dialog = dialogItem.open(component, {
      data: JSON.parse(JSON.stringify(item))
    });
    dialog.afterClosed().subscribe(async result => {
      if (result) {
        result.id = item.id;
        let editResult = await this.service.edit(result);
        let index = this.dataSource.data.indexOf(item);
        this.dataSource.data.fill(editResult, index, index + 1);
        this.dataSource._updateChangeSubscription();
        this.addLog('edited ' + type, editResult);
      }
    });
  }

  async openDialogForGrouping(dialogItem: MatDialog){
    let groupedItems = await this.service.groupBy();
    let dialog = dialogItem.open(GroupDataComponent, {
      data: groupedItems
    });
  }

  delete(item: any) {
    this.service.delete(item.id);
    this.dataSource.data.splice(this.dataSource.data.indexOf(item), 1);
    this.dataSource._updateChangeSubscription();
  }

  addLog(action: string, data: any) {
    let log = {
      userName: JSON.parse(localStorage.getItem('user') as string).userName,
      action: action,
      serializedData: JSON.stringify(data)
    } as Log;
    log.serializedData = JSON.parse(log.serializedData);
    log.actionDate = new Date().toUTCString();
    this.logs.unshift(log);
  }
}
