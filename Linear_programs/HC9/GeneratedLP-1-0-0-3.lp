/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 0.0 ri + -0.02381 ci + -0.87568 si + 0.0 ti + 0.03333 fTempIi + 0.02362 ami + -9.27092;
asi = 0.0 hi + 0.0 li + -0.0 ri + 0.0 ci + -0.0 si + 0.0 ti + 0.0 fTempIi + 0.00043 ami + -0.0;
aTempOi = 0.0 hi + 0.0 li + -0.0 ri + -0.33111 ci + -0.06523 si + 0.0 ti + 0.72464 fTempIi + -0.00148 ami + 9.97909;
fpdi = 0.0 hi + 0.0 li + -0.0 ri + -161.86587 ci + -0.04429 si + 0.0 ti + -0.91204 fTempIi + 0.00091 ami + 582.62541;
fsi = 0.0 hi + 0.0 li + -0.0 ri + -0.52 ci + -0.0 si + 0.0 ti + 0.0 fTempIi + 0.0 ami + 2.59;
fTempOi = 0.0 hi + -0.0 li + 0.0 ri + 0.33175 ci + 0.06291 si + 0.0 ti + 0.31129 fTempIi + -0.00128 ami + 5.88594;
pi = 0.0 hi + -0.0 li + 0.0 ri + 3.66667 ci + -2.57143 si + 0.0 ti + -0.0 fTempIi + -0.0 ami + 591.7619;

apdo = 0.0 ho + 0.0 lo + 9.5246 ro + 0.03915 co + -1.73844 so + -0.0 to + 0.10993 fTempIo + 0.0478 amo + -134.80778;
aso = 0.0 ho + 0.0 lo + 0.0 ro + -0.0 co + 0.0 so + -0.0 to + 0.0 fTempIo + 0.00043 amo + -0.0;
aTempOo = 0.0 ho + -0.0 lo + -0.22006 ro + 0.26313 co + 0.03777 so + -0.0 to + 0.75518 fTempIo + 0.00167 amo + -1.04427;
fpdo = 0.0 ho + -0.0 lo + 2.74159 ro + -14.80601 co + 0.00629 so + -0.0 to + -0.20529 fTempIo + -0.00026 amo + 92.55785;
fso = 0.0 ho + -0.0 lo + -0.0 ro + -0.094 co + 0.0 so + -0.0 to + 4e-05 fTempIo + 0.0 amo + 1.11132;
fTempOo = 0.0 ho + -0.0 lo + 0.22163 ro + -0.26349 co + -0.03744 so + -0.0 to + 0.27402 fTempIo + 0.00154 amo + 12.41898;
po = 0.0 ho + 0.0 lo + 92.66667 ro + 3.56667 co + -6.78476 so + -0.0 to + 0.0 fTempIo + 0.0 amo + 108.5619;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s25i + s30i + s40i;
si =25s25i + 30s30i + 40s40i;
ti = 2;
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
ri < 8;
ci > 1;
ci < 4;
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
ro > 10;
ro < 14;
co > 4;
co < 60;
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
