/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + -0.0 li + 10.33581 ri + -0.01012 ci + -1.47476 si + 0.0 ti + 0.10041 fTempIi + 0.01014 ami + -116.63833;
asi = 0.0 hi + 0.0 li + -0.0 ri + -0.0 ci + 0.0 si + 0.0 ti + 0.0 fTempIi + 0.00011 ami + -0.01286;
aTempOi = 0.0 hi + -0.0 li + 0.29225 ri + -0.07381 ci + -0.04338 si + 0.0 ti + 0.77993 fTempIi + -0.00034 ami + 6.74381;
fpdi = 0.0 hi + 0.0 li + 22.754 ri + -61.12875 ci + -0.03983 si + 0.0 ti + -1.20833 fTempIi + 0.00032 ami + 635.75141;
fsi = 0.0 hi + 0.0 li + -5e-05 ri + -0.14 ci + 1e-05 si + 0.0 ti + 9e-05 fTempIi + -0.0 ami + 2.81249;
fTempOi = 0.0 hi + -0.0 li + -0.28718 ri + 0.07298 ci + 0.04252 si + -0.0 ti + 0.24358 fTempIi + -0.00032 ami + 9.85004;
pi = 0.0 hi + -0.0 li + 257.30833 ri + 2.75 ci + -24.54933 si + 0.0 ti + 0.0 fTempIi + 0.0 ami + 1130.30333;

apdo = 0.0 ho + 0.0 lo + 12.48492 ro + -0.0127 co + -1.8832 so + 0.0 to + 0.12463 fTempIo + 0.012 amo + -154.37514;
aso = 0.0 ho + -0.0 lo + -9e-05 ro + -0.00035 co + -3e-05 so + -0.0 to + -0.00015 fTempIo + 0.00011 amo + -0.0047;
aTempOo = 0.0 ho + 0.0 lo + -0.29466 ro + 0.07987 co + 0.05803 so + 0.0 to + 0.77564 fTempIo + 0.00037 amo + -0.90507;
fpdo = 0.0 ho + -0.0 lo + 25.03198 ro + -72.1273 co + 0.0585 so + -0.0 to + -1.3379 fTempIo + -0.00039 amo + 710.53855;
fso = 0.0 ho + -0.0 lo + 4e-05 ro + -0.15979 co + -1e-05 so + -0.0 to + 9e-05 fTempIo + 0.0 amo + 2.99628;
fTempOo = 0.0 ho + 0.0 lo + 0.28617 ro + -0.07968 co + -0.05691 so + 0.0 to + 0.2366 fTempIo + 0.00035 amo + 12.662;
po = 0.0 ho + 0.0 lo + 227.8 ro + 2.66667 co + -22.79286 so + 0.0 to + -0.0 fTempIo + -0.0 amo + 713.57381;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s30i + s40i + s50i + s60i;
si =30s30i + 40s40i + 50s50i + 60s60i;
ti = 3;
apdi > 100;
apdi < 300;
asi > 0;
asi < 3;
aTempOi > 5;
aTempOi < 25;
fpdi > 100;
fpdi < 300;
fsi > 0;
fsi < 1.25;
fTempOi > 5;
fTempOi < 25;
pi > 0;
ri > 6;
ri < 14;
ci > 1;
ci < 12;
si > 25;
si < 60;
ti > 1;
ti < 3;
fTempIi > 5;
fTempIi < 25;

/* aTempOo < 11.4; */
1 =r6o + r8o + r10o + r12o + r14o;
ro =6r6o + 8r8o + 10r10o + 12r12o + 14r14o;
1 =s25o + s30o + s40o;
so =25s25o + 30s30o + 40s40o;
to = 1;
apdo > 100;
apdo < 300;
aso > 0;
aso < 3;
aTempOo > 5;
aTempOo < 25;
fpdo > 100;
fpdo < 300;
fso > 0;
fso < 1.25;
fTempOo > 5;
fTempOo < 25;
po > 0;
ro > 6;
ro < 14;
co > 1;
co < 11;
so > 25;
so < 60;
to > 1;
to < 3;
fTempIo > 5;
fTempIo < 25;
int hi,li,si,ti,ci,ri,ami;
int ho,lo,so,to,co,ro,amo;
bin s25i,s30i,s40i,s50i,s60i,r6i,r8i,r10i,r12i,r14i;
bin s25o,s30o,s40o,s50o,s60o,r6o,r8o,r10o,r12o,r14o;
