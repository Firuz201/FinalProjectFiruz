/* admin.js — Shop Admin Panel */
"use strict";

/* ─── CHART ─────────────────────────────────────────────── */
(function drawChart() {
  const svg = document.getElementById('revenueChart');
  if (!svg) return;

  const W = svg.parentElement.offsetWidth || 620;
  const H = 180;
  svg.setAttribute('viewBox', `0 0 ${W} ${H}`);

  // Mock data: 12 weeks revenue + orders
  const revenue = [14,21,18,29,24,35,27,42,38,46,41,52];
  const orders  = [8, 12,10,16,14,19,15,24,21,27,23,30];

  function toPath(data, height, pad=20) {
    const max = Math.max(...data);
    const xs = data.map((_,i) => pad + (i / (data.length-1)) * (W - pad*2));
    const ys = data.map(v => height - pad - (v/max)*(height - pad*2));
    let d = `M ${xs[0]} ${ys[0]}`;
    for (let i=1; i<xs.length; i++) {
      const cx = (xs[i-1]+xs[i])/2;
      d += ` C ${cx} ${ys[i-1]}, ${cx} ${ys[i]}, ${xs[i]} ${ys[i]}`;
    }
    return { d, xs, ys };
  }

  function toArea(data, height, pad=20) {
    const { d, xs, ys } = toPath(data, height, pad);
    return `${d} L ${xs[xs.length-1]} ${height-pad} L ${xs[0]} ${height-pad} Z`;
  }

  const { d: rPath, xs, ys: rYs } = toPath(revenue, H);
  const { ys: oYs } = toPath(orders, H);
  const rArea = toArea(revenue, H);
  const oArea = toArea(orders, H);

  // Grid lines
  for (let i=0; i<=4; i++) {
    const y = 20 + (i/4)*(H-40);
    const line = document.createElementNS('http://www.w3.org/2000/svg','line');
    line.setAttribute('x1','0'); line.setAttribute('x2',W);
    line.setAttribute('y1',y); line.setAttribute('y2',y);
    line.setAttribute('stroke','#2a2a24'); line.setAttribute('stroke-width','1');
    svg.insertBefore(line, svg.firstChild);
  }

  // Gradient defs
  const defs = document.createElementNS('http://www.w3.org/2000/svg','defs');
  defs.innerHTML = `
    <linearGradient id="grad-r" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0%" stop-color="#c8870a" stop-opacity="0.35"/>
      <stop offset="100%" stop-color="#c8870a" stop-opacity="0.02"/>
    </linearGradient>
    <linearGradient id="grad-o" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0%" stop-color="#4a7a9b" stop-opacity="0.25"/>
      <stop offset="100%" stop-color="#4a7a9b" stop-opacity="0.02"/>
    </linearGradient>`;
  svg.appendChild(defs);

  // Areas
  [{ area: rArea, fill: 'url(#grad-r)' }, { area: oArea, fill: 'url(#grad-o)' }].forEach(({ area, fill }) => {
    const p = document.createElementNS('http://www.w3.org/2000/svg','path');
    p.setAttribute('d', area); p.setAttribute('fill', fill);
    svg.appendChild(p);
  });

  // Lines
  [{ path: rPath, stroke: '#c8870a' }, { path: toPath(orders,H).d, stroke: '#4a7a9b' }].forEach(({ path, stroke }) => {
    const p = document.createElementNS('http://www.w3.org/2000/svg','path');
    p.setAttribute('d', path); p.setAttribute('fill','none');
    p.setAttribute('stroke', stroke); p.setAttribute('stroke-width','2');
    p.setAttribute('stroke-linecap','round');
    svg.appendChild(p);
  });

  // Dots on last point
  [[xs[xs.length-1], rYs[rYs.length-1], '#c8870a'],
   [xs[xs.length-1], oYs[oYs.length-1], '#4a7a9b']].forEach(([cx,cy,fill]) => {
    const c = document.createElementNS('http://www.w3.org/2000/svg','circle');
    c.setAttribute('cx',cx); c.setAttribute('cy',cy); c.setAttribute('r','4');
    c.setAttribute('fill',fill); c.setAttribute('stroke','#1a1a16'); c.setAttribute('stroke-width','2');
    svg.appendChild(c);
  });

  // X-axis labels
  const months = ['Oct','Nov','Dec','Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep'];
  xs.forEach((x,i) => {
    const t = document.createElementNS('http://www.w3.org/2000/svg','text');
    t.setAttribute('x', x); t.setAttribute('y', H-4);
    t.setAttribute('text-anchor','middle');
    t.setAttribute('font-size','9'); t.setAttribute('fill','#7a7a68');
    t.setAttribute('font-family','DM Mono, monospace');
    t.textContent = months[i];
    svg.appendChild(t);
  });
})();

/* ─── FILTER PILLS ───────────────────────────────────────── */
document.querySelectorAll('.filter-pill').forEach(pill => {
  pill.addEventListener('click', function() {
    document.querySelectorAll('.filter-pill').forEach(p => p.classList.remove('active'));
    this.classList.add('active');
  });
});

/* ─── NAV ITEMS ──────────────────────────────────────────── */
document.querySelectorAll('.nav-item').forEach(item => {
  item.addEventListener('click', function() {
    document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
    this.classList.add('active');
  });
});

/* ─── SIDEBAR MOBILE TOGGLE ──────────────────────────────── */
const menuBtn = document.getElementById('menuToggle');
const sidebar = document.querySelector('.sidebar');
if (menuBtn && sidebar) {
  menuBtn.addEventListener('click', () => sidebar.classList.toggle('open'));
}

/* ─── ANIMATE BARS ON LOAD ───────────────────────────────── */
window.addEventListener('load', () => {
  document.querySelectorAll('.cat-bar-fill').forEach(bar => {
    const w = bar.dataset.width;
    setTimeout(() => { bar.style.width = w; }, 200);
  });
});
