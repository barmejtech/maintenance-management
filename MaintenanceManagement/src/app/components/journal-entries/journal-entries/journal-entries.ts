import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JournalEntryService } from '../../../services/journal-entry.service';
import { GlAccountService } from '../../../services/gl-account.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { Account, CreateJournalEntryRequest, CreateJournalLineItemRequest, JournalEntry } from '../../../models';

@Component({
  selector: 'app-journal-entries',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './journal-entries.html',
  styleUrls: ['./journal-entries.css']
})
export class JournalEntriesComponent implements OnInit {
  entries = signal<JournalEntry[]>([]);
  accounts = signal<Account[]>([]);
  isLoading = signal(false);
  isSaving = signal(false);
  showModal = signal(false);
  isEditing = signal(false);
  readonly today = new Date().toISOString().slice(0, 10);

  form: CreateJournalEntryRequest = this.emptyForm();
  private editingId = '';

  constructor(
    private service: JournalEntryService,
    private accountService: GlAccountService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
    this.accountService.getAll().subscribe({ next: accounts => this.accounts.set(accounts) });
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: entries => {
        this.entries.set(entries);
        this.isLoading.set(false);
      },
      error: () => {
        this.toast.error();
        this.isLoading.set(false);
      }
    });
  }

  openAdd(): void {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = this.emptyForm();
    this.showModal.set(true);
  }

  openEdit(entry: JournalEntry): void {
    this.isEditing.set(true);
    this.editingId = entry.id;
    this.form = {
      entryDate: entry.entryDate.slice(0, 10),
      description: entry.description || '',
      lineItems: entry.lineItems.map(item => ({
        accountId: item.accountId,
        debit: item.debit,
        credit: item.credit,
        description: item.description || ''
      }))
    };
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  addLine(): void {
    this.form.lineItems.push({ accountId: '', debit: 0, credit: 0, description: '' });
  }

  removeLine(index: number): void {
    if (this.form.lineItems.length === 1) {
      return;
    }
    this.form.lineItems.splice(index, 1);
  }

  save(): void {
    if (!this.form.lineItems.length || this.totalDebit === 0 || this.totalDebit !== this.totalCredit) {
      this.toast.show('Journal entry must be balanced before saving.', 'warning');
      return;
    }

    const payload: CreateJournalEntryRequest = {
      entryDate: this.form.entryDate,
      description: this.form.description?.trim() || undefined,
      lineItems: this.form.lineItems.map(item => ({
        accountId: item.accountId,
        debit: Number(item.debit) || 0,
        credit: Number(item.credit) || 0,
        description: item.description?.trim() || undefined
      }))
    };

    this.isSaving.set(true);
    const request = this.isEditing() ? this.service.update(this.editingId, payload) : this.service.create(payload);
    request.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.load();
        this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created');
      },
      error: err => {
        this.isSaving.set(false);
        this.toast.show(err?.error?.message || this.translation.translate('messages.error'), 'error');
      }
    });
  }

  post(entry: JournalEntry): void {
    this.service.post(entry.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.updated');
      },
      error: err => this.toast.show(err?.error?.message || this.translation.translate('messages.error'), 'error')
    });
  }

  delete(entry: JournalEntry): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }
    this.service.delete(entry.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.deleted');
      },
      error: err => this.toast.show(err?.error?.message || this.translation.translate('messages.error'), 'error')
    });
  }

  lineLabel(accountId: string): string {
    const account = this.accounts().find(item => item.id === accountId);
    return account ? `${account.accountCode} - ${account.name}` : 'Select account';
  }

  get totalDebit(): number {
    return this.form.lineItems.reduce((sum, item) => sum + (Number(item.debit) || 0), 0);
  }

  get totalCredit(): number {
    return this.form.lineItems.reduce((sum, item) => sum + (Number(item.credit) || 0), 0);
  }

  getEntryTotal(entry: JournalEntry): number {
    return entry.lineItems.reduce((sum, item) => sum + item.debit, 0);
  }

  private emptyForm(): CreateJournalEntryRequest {
    return {
      entryDate: this.today,
      description: '',
      lineItems: [{ accountId: '', debit: 0, credit: 0, description: '' }]
    };
  }
}

