import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GlAccountService } from '../../../services/gl-account.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { Account, AccountBalance, AccountType, CreateAccountRequest, TrialBalance, UpdateAccountRequest } from '../../../models';

@Component({
  selector: 'app-accounts-gl',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './accounts-gl.html',
  styleUrls: ['./accounts-gl.css']
})
export class AccountsComponent implements OnInit {
  accounts = signal<Account[]>([]);
  trialBalance = signal<TrialBalance | null>(null);
  balanceDetails = signal<AccountBalance | null>(null);
  isLoading = signal(false);
  isSaving = signal(false);
  showModal = signal(false);
  showBalanceModal = signal(false);
  isEditing = signal(false);
  readonly accountTypes = Object.values(AccountType).filter(value => typeof value === 'number') as AccountType[];
  readonly today = new Date().toISOString().slice(0, 10);

  filter = { asOfDate: this.today };
  form: CreateAccountRequest & { isActive: boolean } = this.createEmptyForm();
  private editingId = '';

  constructor(
    private service: GlAccountService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
    this.loadTrialBalance();
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: accounts => {
        this.accounts.set(accounts);
        this.isLoading.set(false);
      },
      error: () => {
        this.toast.error();
        this.isLoading.set(false);
      }
    });
  }

  loadTrialBalance(): void {
    this.service.getTrialBalance(this.filter.asOfDate || undefined).subscribe({
      next: report => this.trialBalance.set(report),
      error: () => this.toast.error()
    });
  }

  openAdd(): void {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = this.createEmptyForm();
    this.showModal.set(true);
  }

  openEdit(account: Account): void {
    this.isEditing.set(true);
    this.editingId = account.id;
    this.form = {
      accountCode: account.accountCode,
      name: account.name,
      type: account.type,
      openingBalance: account.openingBalance,
      openingBalanceDate: account.openingBalanceDate?.slice(0, 10),
      parentAccountId: account.parentAccountId,
      description: account.description,
      isActive: account.isActive
    };
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  save(): void {
    if (!this.form.name.trim()) {
      return;
    }

    this.isSaving.set(true);
    const request = this.isEditing()
      ? this.service.update(this.editingId, {
          name: this.form.name.trim(),
          isActive: this.form.isActive,
          description: this.form.description?.trim() || undefined
        } satisfies UpdateAccountRequest)
      : this.service.create({
          accountCode: this.form.accountCode.trim(),
          name: this.form.name.trim(),
          type: this.form.type,
          openingBalance: this.form.openingBalance || undefined,
          openingBalanceDate: this.form.openingBalanceDate || undefined,
          parentAccountId: this.form.parentAccountId || undefined,
          description: this.form.description?.trim() || undefined
        } satisfies CreateAccountRequest);

    request.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.load();
        this.loadTrialBalance();
        this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created');
      },
      error: err => {
        this.isSaving.set(false);
        this.toast.show(err?.error?.message || this.translation.translate('messages.error'), 'error');
      }
    });
  }

  delete(id: string): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }

    this.service.delete(id).subscribe({
      next: () => {
        this.load();
        this.loadTrialBalance();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  viewBalance(account: Account): void {
    this.service.getBalance(account.id).subscribe({
      next: balance => {
        this.balanceDetails.set(balance);
        this.showBalanceModal.set(true);
      },
      error: () => this.toast.error()
    });
  }

  closeBalance(): void {
    this.showBalanceModal.set(false);
  }

  getTypeLabel(type: AccountType): string {
    return AccountType[type] ?? 'Unknown';
  }

  private createEmptyForm(): CreateAccountRequest & { isActive: boolean } {
    return {
      accountCode: '',
      name: '',
      type: AccountType.Asset,
      openingBalance: undefined,
      openingBalanceDate: this.today,
      parentAccountId: undefined,
      description: undefined,
      isActive: true
    };
  }
}

