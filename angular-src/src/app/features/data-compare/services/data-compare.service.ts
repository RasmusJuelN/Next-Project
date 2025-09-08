import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { Data } from '@angular/router';
import { DataCompare } from '../models/data-compare.model';

@Injectable({
  providedIn: 'root'
})
export class DataCompareService {
  private apiUrl = `${environment.apiUrl}/active-questionnaire`;
  private apiService = inject(ApiService);

  canGetData(studentId:string, templateId:string){
    return this.apiService.get<Array<DataCompare>>(`${this.apiUrl}/${studentId},${templateId}/getresponsesfromuserandtemplate`);
  }


}

