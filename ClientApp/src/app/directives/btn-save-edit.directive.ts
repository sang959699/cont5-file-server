import { Directive, HostListener, HostBinding, ElementRef } from '@angular/core';
import { TextEditorComponent } from '../components/text-editor/text-editor.component';

@Directive({
  selector: '[appBtnSaveEdit]'
})
export class BtnSaveEditDirective {

  constructor(private el: ElementRef, private host: TextEditorComponent) { }
  
  private isConfirmState: boolean = false;

  ngOnInit() {
    this.innerText = this.el.nativeElement.innerText;
    this.classList = this.el.nativeElement.classList;
    this.host.fileContentSubject.subscribe(res => {
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
    this.innerText = 'Save';
    this.isConfirmState = false;
    this.classList.remove('btn-outline-warning');
    this.classList.add('btn-outline-primary');
  }
}
