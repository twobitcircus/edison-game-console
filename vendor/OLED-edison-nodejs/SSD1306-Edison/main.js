//The OLED Display code is adapted from the Adafruit GFX library for Arduino
var sys = require('util');
var SSD1306 = require('./ssd1306.js');
var AFGFX = require('./Adafruit_GFX.js');
var fs = require('fs');
var Edison = require('./Edison.js');
var rootDir = __dirname+'/images/';
var imageList = [];
var lcdTest = null;
var picIndex = 0;
var slideShowTimer = 0;

Edison.enable_i2c6_breakout(startLCD);

function startLCD()
{
  lcdTest = new SSD1306();
  lcdTest.init();
  lcdTest.clear();
  lcdTest.display();
  
  //make an array of all the image in the images directory
  listImages();

  //start the slide show
  slideShow();
}

function slideShow() {
  displayPNG(rootDir + imageList[picIndex]);
  picIndex++;
  if (picIndex >= imageList.length) {
    picIndex = 0;
  }
  clearTimeout(slideShowTimer);
  slideShowTimer = setTimeout(slideShow, 5000);
}


function listImages() {
  var fileList = fs.readdirSync(rootDir);
  for (var i in fileList) {
    var fileName = fileList[i];

    if (fileName.match(/.+\.png$/i)) {
      console.log(fileName);
      imageList.push(fileName);
    }
  }
}

function displayPNG(fullPathToPNG) {
  var PNG = require('pngjs').PNG;

  fs.createReadStream(fullPathToPNG)
    .pipe(new PNG({
      filterType: 4
    }))
    .on('parsed', function () {
      lcdTest.clear();
      for (var y = 0; y < this.height; y++) {
        for (var x = 0; x < this.width; x++) {
          var idx = (this.width * y + x) << 2;
          // invert color
          if (this.data[idx] == 0 && this.data[idx + 1] == 0 && this.data[idx + 2] == 0) {
            lcdTest.drawPixel(x, y, 1);
          } else {
            lcdTest.drawPixel(x, y, 0);
          }
        }
      }
      lcdTest.display();
    });
}