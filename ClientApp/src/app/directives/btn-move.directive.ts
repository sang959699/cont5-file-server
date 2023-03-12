import { Directive, HostListener, HostBinding, ElementRef } from '@angular/core';
import { FileListComponent } from '../components/file-list/file-list.component';

@Directive({
  selector: '[appBtnMove]'
})
export class BtnMoveDirective {

  constructor(private el: ElementRef, private host: FileListComponent) { }
  
  private isConfirmState: boolean = false;

  ngOnInit() {
    this.innerText = this.el.nativeElement.innerText;
    this.classList = this.el.nativeElement.classList;
    this.disabled = true;
    this.host.movePathSubject.subscribe(res => {
      this.disabled = (res == null);
      this.resetMoveState();
    });
  }

  @HostBinding('innerText') innerText: string;
  @HostBinding('disabled') disabled: boolean;
  @HostBinding('classList') classList: DOMTokenList;

  @HostListener("click", ["$event"])
  public onClick(event: any): void{
    if (!this.isConfirmState) {
      this.setConfirmState();
    }
  }

  setConfirmState = () => {
    event.preventDefault();
    this.innerText = 'Confirm';
    this.isConfirmState = true;
    this.classList.remove('btn-outline-primary');
    this.classList.add('btn-outline-warning');
  }

  resetMoveState = () => {
    this.innerText = 'Move';
    this.isConfirmState = false;
    this.classList.remove('btn-outline-warning');
    this.classList.add('btn-outline-primary');
  }
}
