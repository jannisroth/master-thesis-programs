/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 9.42024 ri + -0.03307 ci + -1.70941 si + -0.0 ti + 0.10268 fTempIi + 0.0465 ami + -132.30368;
asi = 0.0 hi + 0.0 li + 0.0 ri + -0.0 ci + 0.0 si + -0.0 ti + 0.0 fTempIi + 0.00043 ami + -0.0;
aTempOi = 0.0 hi + -0.0 li + 0.20471 ri + -0.23446 ci + -0.0351 si + -0.0 ti + 0.80019 fTempIi + -0.0016 ami + 7.09894;
fpdi = 0.0 hi + -0.0 li + 2.80917 ri + -14.88537 ci + -0.00568 si + -0.0 ti + -0.19926 fTempIi + 0.00024 ami + 92.96773;
fsi = 0.0 hi + -0.0 li + -0.0 ri + -0.094 ci + 0.0 si + -0.0 ti + -0.0 fTempIi + -0.0 ami + 1.112;
fTempOi = 0.0 hi + -0.0 li + -0.20238 ri + 0.23595 ci + 0.03413 si + 0.0 ti + 0.2375 fTempIi + -0.00141 ami + 9.66377;
pi = 0.0 hi + 0.0 li + 92.66667 ri + 3.56667 ci + -6.78476 si + -0.0 ti + 0.0 fTempIi + 0.0 ami + 108.5619;

apdo = 0.0 ho + -0.0 lo + 9.54266 ro + 0.02976 co + -1.59443 so + 0.0 to + 0.11173 fTempIo + 0.04383 amo + -123.67275;
aso = 0.0 ho + 0.0 lo + 0.0 ro + 0.0 co + -0.0 so + 0.0 to + 0.0 fTempIo + 0.00043 amo + -0.0;
aTempOo = 0.0 ho + 0.0 lo + -0.2124 ro + 0.24706 co + 0.0472 so + 0.0 to + 0.7972 fTempIo + 0.00162 amo + -1.35407;
fpdo = 0.0 ho + 0.0 lo + 26.69611 ro + -297.28373 co + 0.0574 so + 0.0 to + -1.58667 fTempIo + -0.00225 amo + 766.81389;
fso = 0.0 ho + 0.0 lo + 1e-05 ro + -0.51982 co + -0.0 so + 0.0 to + 8e-05 fTempIo + 0.0 amo + 2.58825;
fTempOo = 0.0 ho + 0.0 lo + 0.2095 ro + -0.24464 co + -0.04602 so + 0.0 to + 0.22954 fTempIo + 0.00168 amo + 12.37932;
po = 0.0 ho + -0.0 lo + 73.26667 ro + 3.66667 co + -5.73929 so + 0.0 to + 0.0 fTempIo + 0.0 amo + 239.06071;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s25i + s30i + s40i;
si =25s25i + 30s30i + 40s40i;
ti = 1;
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
ri > 10;
ri < 14;
ci > 4;
ci < 60;
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
to = 2;
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
ro > 8;
ro < 14;
co > 1;
co < 4;
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
