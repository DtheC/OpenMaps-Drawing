float[] lats = {-37.8132014, -37.8131548, -37.8130501, -37.8134125, -37.8135200, -37.8135669};
float[] lons = {144.9681995, 144.9683603, 144.9687214, 144.9689011, 144.9685284, 144.9683657};

void setup(){
 size(400,400);
 background(0);
 
 float preX = 0;
 float preY = 0;
 
 for (int i=0;i<lats.length;i++){
   fill(255,0,130);
   
   float x = map(abs((lats[i]+37)), 0.813, 0.814, 0, width);
   float y = map(abs((lons[i]-144)), 0.968, 0.969, 0, height);
   
   stroke(255);
   line(x, y, preX, preY);
   
   preX = x;
   preY = y;
   
   println(abs((lons[i]-144)));
   
  ellipse(x, y, 20, 20); 
 }
}