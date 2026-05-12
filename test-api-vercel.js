const http = require('https');

const data = JSON.stringify({
  pin: '1234'
});

const options = {
  hostname: 'life-tag-backend-ahmedrashed2611-5674s-projects.vercel.app',
  port: 443,
  path: '/api/verify-pin',
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Content-Length': Buffer.byteLength(data)
  }
};

const req = http.request(options, (res) => {
  console.log(`STATUS: ${res.statusCode}`);
  let responseData = '';

  res.on('data', (chunk) => {
    responseData += chunk;
  });

  res.on('end', () => {
    console.log('RESPONSE:', responseData);
  });
});

req.on('error', (e) => {
  console.error(`problem with request: ${e.message}`);
});

req.write(data);
req.end();
