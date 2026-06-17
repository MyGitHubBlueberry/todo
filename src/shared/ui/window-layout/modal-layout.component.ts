import { Component, input, output } from "@angular/core";

@Component({
  selector: 'app-modal-layout',
  templateUrl: './modal-layout.html',
})
export class ModalLayoutComponent {
  public readonly title = input.required<string>();
  public readonly onSave = output<void>();
  public readonly onClose = output<void>();
}
