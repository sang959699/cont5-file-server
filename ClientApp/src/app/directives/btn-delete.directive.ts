import { Directive, HostListener, HostBinding, ElementRef } from '@angular/core';

@Directive({
  selector: '[appBtnDelete]'
})
export class BtnDeleteDirective {

  constructor(private el: ElementRef) { }
  
  private isConfirmState: boolean = false;
  private initialInnerText: string;

  ngOnInit() {
    this.innerText = this.el.nativeElement.innerText;
    this.initialInnerText = this.innerText;
  }

  @HostBinding('innerText') innerText: string;
  @HostBinding('disabled') disabled: boolean;

  @HostListener("click", ["$event"])
  public onClick(event: any): void{
    if (!this.isConfirmState) {
      this.setConfirmState();
    } else {
      this.resetDeleteState();
    }
  }

  setConfirmState = () => {
    event.preventDefault();
    this.innerText = 'Confirm';
    this.isConfirmState = true;
  }

  resetDeleteState = () => {
    this.innerText = this.initialInnerText;
    this.isConfirmState = false;
  }
}
