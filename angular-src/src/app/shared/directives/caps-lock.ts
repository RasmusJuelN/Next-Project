import { Directive, EventEmitter, HostListener, Output } from "@angular/core";

@Directive({ 
  selector: '[capsLock]',
  standalone: true 
})
export class TrackCapsDirective {
  @Output('capsLock') capsLock = new EventEmitter<boolean>();

  @HostListener('window:keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    this.capsLock.emit(event.getModifierState && event.getModifierState('CapsLock'));
  }
  @HostListener('window:keyup', ['$event'])
  onKeyUp(event: KeyboardEvent): void {
    this.capsLock.emit(event.getModifierState && event.getModifierState('CapsLock'));
  }
}