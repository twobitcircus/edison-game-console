// Dependencies
var sys = require('sys');
var exec = require('child_process').exec;
var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var fs = require('fs');
var WebSocketServer = require('ws').Server;
var _ = require("underscore");
var child;

var http = require("http");

//----- GAME SERVER --------
//-------------------------
var playerScore = 0;
var gameOver = false;

function resetGameVars() {
  gameOver = false; // Game Over state
  playerScore = 0;  // Default score
}

//-------------------------
//----- WEB SERVER --------
//-------------------------

// Page routes
var routes = require('./routes/index');

var app = express();
// view engine setup
app.use(express.static(__dirname + '/public'));
app.engine('html', require('ejs').renderFile);
app.set('views', __dirname + '/views');
app.set('view engine', 'html');
//app.use(favicon(__dirname + '/public/favicon.ico')); // uncomment after placing your favicon in /public
app.set('env', 'development');//XXX
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use('/', routes);

app.get("/crossdomain.xml", function(req, res) {
  console.log("Fetched crossdomain.xml");
  res.contentType("text/xml");
  res.send('<?xml version="1.0" ?><cross-domain-policy><allow-access-from domain="*" to-ports="1200" /></cross-domain-policy>');
});

// catch 404 and forward to error handler
app.use(function(req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function(err, req, res, next) {
        res.status(err.status || 500);
        res.json({
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.json({
        message: err.message,
        error: {}
    });
});

// Start web server
app.listen(8000, function() {
  console.log("WebServer started (listening on port 8000)...");
  // Reset score
  child = exec("edison-sparkfun-oled/oled_edison_unity", function (error, stdout, stderr) {
    sys.print('stdout: ' + stdout);
    sys.print('stderr: ' + stderr);
    if (error !== null) {
      console.log('exec error: ' + error);
    }
  });

});

// Security Policy server (policy.xml)
var net = require('net');

var server = net.createServer(function(c) { 
  console.log('security policy server running on port 843');
  c.on("data", function(data) {
    data = data.toString();
    if (data.indexOf("<policy-file-request/>") == 0) {
      console.log("sending policy file");
      policy_file = fs.readFileSync("policy.xml");
      c.write(policy_file);
      c.end();
    }
  });
  c.on('end', function() {
    console.log('client disconnected');
  });
  c.pipe(c);
});
server.listen(843, function() { 
    console.log('server bound');
});
// end security policy server


//-------------------------
//------ WebSocket --------
//-------------------------

// WebSocket Server
var wss = new WebSocketServer({ port: 1200 }, function() { // Unity likes ports 1200-1220 (original port = 8080)
  console.log("WebSocketServer started (listening on port 1200)...");
});

wss.on('connection', function connection(ws) {
  console.log('WebSocketServer connection initiated');

  ws.on('message', function incoming(message) {
    console.log('WebSocket message received: %s', message);
    // wss.broadcast(message);

    if (message == "gameover" && !gameOver) {
      gameOver = true;
      console.log("GAMEOVER"); //XXX
      child = exec("edison-sparkfun-oled/oled_edison_unity " + playerScore + " gameover", function (error, stdout, stderr) {
        sys.print('stdout: ' + stdout + '\n');
        sys.print('stderr: ' + stderr + '\n');
        if (error !== null) {
          console.log('exec error: ' + error);
        }
      });
    } else {
      // Update score screen (only when score increases)
      if (message > playerScore && !gameOver) {
        playerScore = message;
        console.log("SCORE: " + message); // XXX
        child = exec("edison-sparkfun-oled/oled_edison_unity " + message, function (error, stdout, stderr) {
          sys.print('stdout: ' + stdout + '\n\r');
          sys.print('stderr: ' + stderr + '\n\r');
          if (error !== null) {
            console.log('exec error: ' + error + '\n\r');
          }
        });
      }
    }
  });

  ws.on('open', function open() {
    console.log('WebSocket connected');
    // ws.send(Date.now().toString(), {mask: true});

    // Start game vars over
    resetGameVars();
  });

  ws.on('close', function close() {
    console.log('WebSocket disconnected');
  });

});

function broadcast(data) {
  console.log("broadcast ", data);
  wss.clients.forEach(function each(client) {
    client.send(data);
  });
}


// Pins (hardware)
gpio_objs = {};
pin_states = {};
pins = {
  a: 49,
  b: 46,
  u: 47,
  d: 44,
  l: 165,
  r: 45,
  s: 48
};

setInterval(function() {
  _.each(_.keys(pins), function(pin_name){
    // Read GPIO kernel files per button
    var filename = "/sys/kernel/debug/gpio_debug/gpio"+pins[pin_name]+"/current_value";
    fs.readFile(filename, function(err, data) {
      var _val;
      if (data.toString().indexOf("high") != -1) {
        _val = false;
      } else {
        _val = true;
      }
      if (pin_states[pin_name] != _val) {
        if (_val) broadcast(pin_name.toUpperCase());
        else broadcast(pin_name.toLowerCase());

        pin_states[pin_name] = _val;
      }
    });
  });
}, 20);

/*
// For testing, forces player message Up ("U")
setInterval(function() {
  console.log("I should be broadcasting 'U' via WebSocketServer");
  broadcast("U");
}, 1000);
*/
