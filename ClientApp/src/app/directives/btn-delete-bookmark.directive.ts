import { Directive, HostListener, HostBinding, ElementRef } from '@angular/core';

@Directive({
  selector: '[appBtnDeleteBookmark]'
})
export class BtnDeleteBookmarkDirective {

  constructor(private el: ElementRef) { }
  
  private isConfirmState: boolean = false;
  private initialClass: string;

  ngOnInit() {
    this.class = this.el.nativeElement.classList.value;
    this.initialClass = this.class;
  }

  @HostBinding('class') class: string;
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
    this.class = 'btn btn-outline-danger py-1 mt-n1';
    this.isConfirmState = true;
  }

  resetDeleteState = () => {
    this.class = this.initialClass;
    this.isConfirmState = false;
  }
}
