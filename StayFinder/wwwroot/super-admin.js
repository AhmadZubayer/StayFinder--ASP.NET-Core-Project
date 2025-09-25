// super-admin.js
// Demo data and UI logic for Super Admin Dashboard

// --- Demo Data ---
const users = [
  {id: 1, name: 'Karim', email: 'karim@mail.com', role: 'host', status: 'active', registered: '2025-01-10'},
  {id: 2, name: 'Rohim', email: 'rohim@mail.com', role: 'host', status: 'suspended', registered: '2025-03-01'},
  {id: 3, name: 'Shakib', email: 'shakib@mail.com', role: 'host', status: 'active', registered: '2025-04-12'},
  {id: 4, name: 'Sumaiya', email: 'sumaiya@mail.com', role: 'customer', status: 'active', registered: '2025-02-15'},
];
const properties = [
  {id: 101, title: 'Dhaka Lake View', host: 'Karim', status: 'pending'},
  {id: 102, title: 'Chittagong Hill Cottage', host: 'Rohim', status: 'approved'},
  {id: 103, title: 'Sylhet Tea Resort', host: 'Shakib', status: 'approved'},
];
const reviews = [
  {id: 201, property: 'Dhaka Lake View', customer: 'Sumaiya', rating: 4.5, comment: 'Great stay!'},
  {id: 202, property: 'Chittagong Hill Cottage', customer: 'Karim', rating: 2.0, comment: 'Needs improvement.'},
];
const earnings = [
  {date: '2025-08-01', host: 'Karim', amount: 500},
  {date: '2025-08-10', host: 'Rohim', amount: 300},
  {date: '2025-09-01', host: 'Shakib', amount: 700},
];
const notifications = [
  {to: 'All Users', message: 'Welcome to the platform!', date: '2025-09-01'},
];

// --- Sidebar Navigation ---
const sections = ['users','properties','reviews','earnings','reports','notifications'];
document.querySelectorAll('.sidebar a').forEach((a, i) => {
  a.addEventListener('click', e => {
    e.preventDefault();
    document.querySelectorAll('.sidebar a').forEach(x => x.classList.remove('active'));
    a.classList.add('active');
    sections.forEach(s => document.getElementById(s).classList.add('hidden'));
    if (sections[i]) document.getElementById(sections[i]).classList.remove('hidden');
  });
});

// --- User Table ---
function renderUsers() {
  const tbody = document.getElementById('userTableBody');
  tbody.innerHTML = users.map(u => `
    <tr>
      <td class="py-2 px-3">${u.id}</td>
      <td class="py-2 px-3">${u.name}</td>
      <td class="py-2 px-3">${u.email}</td>
      <td class="py-2 px-3">${u.role}</td>
      <td class="py-2 px-3">${u.status}</td>
      <td class="py-2 px-3">${u.registered}</td>
      <td class="py-2 px-3 flex gap-2">
        <button class="text-[#04aa6d] underline" onclick="openUserModal('edit',${u.id})">Edit</button>
        <button class="text-red-600 underline" onclick="confirmAction('Remove this user?',()=>removeUser(${u.id}))">Delete</button>
      </td>
    </tr>
  `).join('');
}
renderUsers();

// --- User Modal ---
function openUserModal(mode, id) {
  document.getElementById('userModal').classList.remove('hidden');
  document.getElementById('userModalTitle').textContent = mode==='add' ? 'Add User' : 'Edit User';
  if (mode==='edit') {
    const u = users.find(x=>x.id===id);
    document.getElementById('userIdInput').value = u.id;
    document.getElementById('userNameInput').value = u.name;
    document.getElementById('userEmailInput').value = u.email;
    document.getElementById('userRoleInput').value = u.role;
    document.getElementById('userStatusInput').value = u.status;
  } else {
    document.getElementById('userIdInput').value = '';
    document.getElementById('userNameInput').value = '';
    document.getElementById('userEmailInput').value = '';
    document.getElementById('userRoleInput').value = 'customer';
    document.getElementById('userStatusInput').value = 'active';
  }
}
function closeUserModal() {
  document.getElementById('userModal').classList.add('hidden');
}
document.getElementById('userForm').onsubmit = function(e) {
  e.preventDefault();
  const id = document.getElementById('userIdInput').value;
  const name = document.getElementById('userNameInput').value;
  const email = document.getElementById('userEmailInput').value;
  const role = document.getElementById('userRoleInput').value;
  const status = document.getElementById('userStatusInput').value;
  if (id) {
    const u = users.find(x=>x.id==id);
    u.name = name; u.email = email; u.role = role; u.status = status;
    showMessage('User updated!','success');
  } else {
    users.push({id: Date.now(), name, email, role, status, registered: new Date().toISOString().slice(0,10)});
    showMessage('User added!','success');
  }
  closeUserModal();
  renderUsers();
};
function removeUser(id) {
  const idx = users.findIndex(x=>x.id==id);
  if (idx>-1) users.splice(idx,1);
  renderUsers();
  showMessage('User removed!','success');
}

// --- Property Table ---
function renderProperties() {
  const tbody = document.getElementById('propertyTableBody');
  tbody.innerHTML = properties.map(p => `
    <tr>
      <td class="py-2 px-3">${p.id}</td>
      <td class="py-2 px-3">${p.title}</td>
      <td class="py-2 px-3">${p.host}</td>
      <td class="py-2 px-3">${p.status}</td>
      <td class="py-2 px-3 flex gap-2">
        ${p.status==='pending'?`<button class='text-[#04aa6d] underline' onclick='approveProperty(${p.id})'>Approve</button>`:''}
        <button class="text-red-600 underline" onclick="confirmAction('Remove this property?',()=>removeProperty(${p.id}))">Remove</button>
      </td>
    </tr>
  `).join('');
}
renderProperties();
function approveProperty(id) {
  const p = properties.find(x=>x.id==id);
  if (p) { p.status = 'approved'; renderProperties(); showMessage('Property approved!','success'); }
}
function removeProperty(id) {
  const idx = properties.findIndex(x=>x.id==id);
  if (idx>-1) properties.splice(idx,1);
  renderProperties();
  showMessage('Property removed!','success');
}

// --- Review Table ---
function renderReviews() {
  const tbody = document.getElementById('reviewTableBody');
  tbody.innerHTML = reviews.map(r => `
    <tr>
      <td class="py-2 px-3">${r.id}</td>
      <td class="py-2 px-3">${r.property}</td>
      <td class="py-2 px-3">${r.customer}</td>
      <td class="py-2 px-3">${r.rating}</td>
      <td class="py-2 px-3">${r.comment}</td>
      <td class="py-2 px-3 flex gap-2">
        <button class="text-yellow-600 underline" onclick="sendWarning('${r.property}')">Warn Host</button>
        <button class="text-red-600 underline" onclick="confirmAction('Suspend this host?',()=>suspendHost('${r.property}'))">Suspend Host</button>
      </td>
    </tr>
  `).join('');
}
renderReviews();
function sendWarning(property) {
  showMessage('Warning sent to host of '+property,'success');
}
function suspendHost(property) {
  showMessage('Host of '+property+' suspended!','error');
}

// --- Earnings ---
function renderEarnings() {
  // Total
  const total = earnings.reduce((sum,e)=>sum+e.amount*0.1,0);
  document.getElementById('totalEarnings').textContent = '$'+total.toFixed(2);
  // By host
  const byHost = {};
  earnings.forEach(e=>{byHost[e.host]=(byHost[e.host]||0)+e.amount*0.1;});
  document.getElementById('hostEarningsList').innerHTML = Object.entries(byHost).map(([h,amt])=>`<li>${h}: $${amt.toFixed(2)}</li>`).join('');
  // Chart
  const months = {};
  earnings.forEach(e=>{
    const m = e.date.slice(0,7);
    months[m]=(months[m]||0)+e.amount*0.1;
  });
  const ctx = document.getElementById('incomeChart').getContext('2d');
  new Chart(ctx, {
    type: 'bar',
    data: {
      labels: Object.keys(months),
      datasets: [{label:'Commission',data:Object.values(months),backgroundColor:'#04aa6d'}]
    },
    options: {plugins:{legend:{display:false}},scales:{y:{beginAtZero:true}}}
  });
  // Transactions
  document.getElementById('transactionTableBody').innerHTML = earnings.map(e=>`<tr><td class='py-2 px-3'>${e.date}</td><td class='py-2 px-3'>${e.host}</td><td class='py-2 px-3'>$${e.amount}</td><td class='py-2 px-3'>$${(e.amount*0.1).toFixed(2)}</td></tr>`).join('');
}
renderEarnings();

// --- Reports ---
function renderReports() {
  document.getElementById('activeUsers').textContent = users.filter(u=>u.status==='active').length;
  document.getElementById('totalBookings').textContent = earnings.length;
  document.getElementById('topProperties').innerHTML = properties.slice(0,2).map(p=>`<li>${p.title}</li>`).join('');
  document.getElementById('topHosts').innerHTML = users.filter(u=>u.role==='host').slice(0,2).map(h=>`<li>${h.name}</li>`).join('');
}
renderReports();

// --- Notifications ---
function renderNotifications() {
  document.getElementById('notificationList').innerHTML = notifications.map(n=>`<li><span class='font-bold'>${n.to}:</span> ${n.message} <span class='text-xs text-gray-400'>(${n.date})</span></li>`).join('');
}
renderNotifications();
function openNotificationModal() {
  document.getElementById('notificationModal').classList.remove('hidden');
}
function closeNotificationModal() {
  document.getElementById('notificationModal').classList.add('hidden');
}
document.getElementById('notificationForm').onsubmit = function(e) {
  e.preventDefault();
  const to = document.getElementById('notificationToInput').value;
  const message = document.getElementById('notificationMessageInput').value;
  notifications.unshift({to,message,date:new Date().toISOString().slice(0,10)});
  closeNotificationModal();
  renderNotifications();
  showMessage('Notification sent!','success');
};

// --- Confirmation Modal ---
function confirmAction(msg, cb) {
  document.getElementById('confirmMessage').textContent = msg;
  document.getElementById('confirmModal').classList.remove('hidden');
  document.getElementById('confirmOkBtn').onclick = function() {
    document.getElementById('confirmModal').classList.add('hidden');
    cb();
  };
  document.getElementById('confirmCancelBtn').onclick = function() {
    document.getElementById('confirmModal').classList.add('hidden');
  };
}

// --- Message Box ---
function showMessage(msg, type) {
  const box = document.getElementById('messageBox');
  box.textContent = msg;
  box.className = 'mb-4 p-4 rounded text-white font-semibold ' + (type==='success'?'bg-[#04aa6d]':'bg-red-600');
  box.classList.remove('hidden');
  setTimeout(()=>box.classList.add('hidden'), 2500);
}

// --- Logout ---
function logout() {
  window.location.href = 'login.html';
}

// --- Modal Close on BG Click ---
document.querySelectorAll('.modal-bg').forEach(bg => {
  bg.addEventListener('click', e => { if (e.target===bg) bg.classList.add('hidden'); });
});
