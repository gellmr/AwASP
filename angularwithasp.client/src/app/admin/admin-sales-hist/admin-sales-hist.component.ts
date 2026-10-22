import { Component, AfterViewInit, ViewChild, ElementRef, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-admin-sales-hist',
  standalone: true,
  imports: [CommonModule, AdminTitleBarComponent],
  templateUrl: './admin-sales-hist.component.html',
  styleUrl: './admin-sales-hist.component.css'
})
export class AdminSalesHistComponent implements AfterViewInit, OnDestroy
{
  @ViewChild('historicalChart') historicalChartRef!: ElementRef<HTMLCanvasElement>;
  
  private http = inject(HttpClient);
  private chart: Chart | null = null;

  ngAfterViewInit() {
    // Replace with your historical sales backend endpoint URL
    this.http.get<any[]>('/api/admin-hist-sales').subscribe(data => {
      const labels = data.map(item => item.date);
      const values = data.map(item => item.totalSales);

      const ctx = this.historicalChartRef.nativeElement.getContext('2d');
      if (ctx) {
        this.chart = new Chart(ctx, {
          type: 'line',
          data: {
            labels,
            datasets: [{
              label: 'Historical Sales',
              data: values,
              borderColor: '#36a2eb',
              backgroundColor: 'rgba(54, 162, 235, 0.3)',
              fill: true,
              tension: 0.1,
              pointRadius: 0
            }]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { display: false }
            },
            scales: {
              x: { 
                ticks: { color: 'white', maxTicksLimit: 8 }, 
                grid: { color: 'rgba(255,255,255,0.05)' } 
              },
              y: { 
                ticks: { color: 'white' }, 
                grid: { color: 'rgba(255,255,255,0.1)' } 
              }
            }
          }
        });
      }
    });
  }

  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }
}