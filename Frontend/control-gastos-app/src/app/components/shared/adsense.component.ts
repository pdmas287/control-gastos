import { Component, Input, OnInit, AfterViewInit, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-adsense',
  standalone: true,
  template: `
    <div class="ad-container">
      <ins class="adsbygoogle"
           [style.display]="'block'"
           [attr.data-ad-client]="adClient"
           [attr.data-ad-slot]="adSlot"
           [attr.data-ad-format]="adFormat"
           [attr.data-full-width-responsive]="fullWidthResponsive"></ins>
    </div>
  `,
  styles: [`
    .ad-container {
      margin: 20px 0;
      text-align: center;
      min-height: 90px;
      background-color: #f5f5f5;
      border: 1px dashed #ddd;
      border-radius: 4px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .ad-container::before {
      content: 'Publicidad';
      color: #999;
      font-size: 12px;
      position: absolute;
      top: 5px;
      left: 50%;
      transform: translateX(-50%);
    }
  `]
})
export class AdsenseComponent implements OnInit, AfterViewInit {
  @Input() adClient: string = 'ca-pub-1445179381452360'; // Tu ID de AdSense
  @Input() adSlot: string = ''; // ID del slot del anuncio
  @Input() adFormat: string = 'auto'; // auto, rectangle, horizontal, vertical
  @Input() fullWidthResponsive: string = 'true';

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  ngOnInit(): void {
    // Inicialización si es necesario
  }

  ngAfterViewInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      try {
        // Cargar el script de AdSense si aún no está cargado
        ((window as any).adsbygoogle = (window as any).adsbygoogle || []).push({});
      } catch (e) {
        console.error('Error loading AdSense:', e);
      }
    }
  }
}
