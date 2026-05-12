import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { CreateStrataRollDto, StrataRollDto, StrataRollStatus, UpdateStrataRollDto } from '../models';

@Injectable({ providedIn: 'root' })
export class StrataRollService {
  // TODO(api): Replace this in-memory placeholder with backend API integration once /strata-rolls endpoints are available.
  private rolls: StrataRollDto[] = [];

  getAll(): Observable<StrataRollDto[]> {
    return of([...this.rolls]);
  }

  create(dto: CreateStrataRollDto): Observable<StrataRollDto> {
    const created: StrataRollDto = {
      ...dto,
      id: this.newId(),
      status: dto.status ?? StrataRollStatus.Draft,
      createdAt: new Date().toISOString(),
      unitNumber: ''
    };
    this.rolls = [created, ...this.rolls];
    return of(created);
  }

  update(id: string, dto: UpdateStrataRollDto): Observable<StrataRollDto> {
    const existing = this.rolls.find(r => r.id === id);
    const updated: StrataRollDto = {
      ...existing,
      ...dto,
      id,
      createdAt: existing?.createdAt ?? new Date().toISOString(),
      unitNumber: existing?.unitNumber ?? ''
    } as StrataRollDto;
    this.rolls = this.rolls.map(r => r.id === id ? updated : r);
    return of(updated);
  }

  delete(id: string): Observable<void> {
    this.rolls = this.rolls.filter(r => r.id !== id);
    return of(void 0);
  }

  seedUnitLabels(unitLabels: Record<string, string>): void {
    this.rolls = this.rolls.map(roll => ({
      ...roll,
      unitNumber: unitLabels[roll.unitId] ?? roll.unitNumber
    }));
  }

  private newId(): string {
    return crypto.randomUUID();
  }
}
