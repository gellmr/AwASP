import { Component, AfterViewInit, ViewChild, ElementRef, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-admin-sales-prod',
  standalone: true,
  imports: [CommonModule, AdminTitleBarComponent],
  templateUrl: './admin-sales-prod.component.html',
  styleUrl: './admin-sales-prod.component.css'
})
export class AdminSalesProdComponent implements AfterViewInit, OnDestroy {
  @ViewChild('revenueBarChart') revenueBarRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('revenuePieChart') revenuePieRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('quantityBarChart') quantityBarRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('quantityPieChart') quantityPieRef!: ElementRef<HTMLCanvasElement>;

  private http = inject(HttpClient);
  private charts: Chart[] = [];

  ngAfterViewInit() {
    this.http.get<any[]>('/api/admin-sales-bi').subscribe(data => {
      const labels = data.map(item => item.productTitle);
      const revenues = data.map(item => item.totalRevenue);
      const quantities = data.map(item => item.quantitySold);

      this.renderRevenueBar(labels, revenues);
      this.renderRevenuePie(labels, revenues);
      this.renderQuantityBar(labels, quantities);
      this.renderQuantityPie(labels, quantities);
    });
  }

  private renderRevenueBar(labels: string[], data: number[]) {
    const ctx = this.revenueBarRef.nativeElement.getContext('2d');
    if (ctx) {
      this.charts.push(new Chart(ctx, {
        type: 'bar',
        data: {
          labels,
          datasets: [{ label: 'Revenue ($)', data, backgroundColor: '#36a2eb' }]
        },
        options: { responsive: true, plugins: { legend: { display: false } }, scales: { x: { ticks: { color: 'white' } }, y: { ticks: { color: 'white' } } } }
      }));
    }
  }

  private renderRevenuePie(labels: string[], data: number[]) {
    const ctx = this.revenuePieRef.nativeElement.getContext('2d');
    if (ctx) {
      this.charts.push(new Chart(ctx, {
        type: 'doughnut',
        data: {
          labels,
          datasets: [{ data, backgroundColor: ['#36a2eb', '#ff6384', '#cc65fe', '#ffce56', '#4bc0c0', '#9966ff'] }]
        },
        options: { responsive: true, plugins: { legend: { position: 'bottom', labels: { color: 'white', boxWidth: 12 } } } }
      }));
    }
  }

  private renderQuantityBar(labels: string[], data: number[]) {
    const ctx = this.quantityBarRef.nativeElement.getContext('2d');
    if (ctx) {
      this.charts.push(new Chart(ctx, {
        type: 'bar',
        data: {
          labels,
          datasets: [{ label: 'Quantity', data, backgroundColor: '#4bc0c0' }]
        },
        options: { responsive: true, plugins: { legend: { display: false } }, scales: { x: { ticks: { color: 'white' } }, y: { ticks: { color: 'white' } } } }
      }));
    }
  }

  private renderQuantityPie(labels: string[], data: number[]) {
    const ctx = this.quantityPieRef.nativeElement.getContext('2d');
    if (ctx) {
      this.charts.push(new Chart(ctx, {
        type: 'doughnut',
        data: {
          labels,
          datasets: [{ data, backgroundColor: ['#4bc0c0', '#36a2eb', '#ff6384', '#ffce56', '#cc65fe', '#9966ff'] }]
        },
        options: { responsive: true, plugins: { legend: { position: 'bottom', labels: { color: 'white', boxWidth: 12 } } } }
      }));
    }
  }

  ngOnDestroy() {
    this.charts.forEach(chart => chart.destroy());
  }
}
