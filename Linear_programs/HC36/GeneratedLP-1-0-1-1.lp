/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 11.80595 ri + -0.00871 ci + -1.78629 si + 0.0 ti + 0.11338 fTempIi + 0.0096 ami + -145.28069;
asi = 0.0 hi + 0.0 li + -0.0 ri + -0.0 ci + 0.0 si + -0.0 ti + -0.0 fTempIi + 0.0001 ami + -0.00214;
aTempOi = 0.0 hi + 0.0 li + 0.29235 ri + -0.06581 ci + -0.05368 si + -0.0 ti + 0.77769 fTempIi + -0.00031 ami + 7.01713;
fpdi = 0.0 hi + -0.0 li + 13.13333 ri + -24.3619 ci + -0.02976 si + -0.0 ti + -0.71097 fTempIi + 0.00015 ami + 367.78712;
fsi = 0.0 hi + -0.0 li + -0.0001 ri + -0.07933 ci + 2e-05 si + 0.0 ti + 0.00036 fTempIi + -0.0 ami + 2.27741;
fTempOi = 0.0 hi + 0.0 li + -0.29078 ri + 0.06588 ci + 0.05343 si + 0.0 ti + 0.23478 fTempIi + -0.00026 ami + 9.63713;
pi = 0.0 hi + 0.0 li + 244.38333 ri + 3.50286 ci + -19.51143 si + 0.0 ti + -0.0 fTempIi + 0.0 ami + 630.40381;

apdo = 0.0 ho + 0.0 lo + 11.94056 ro + 0.0081 co + -1.80322 so + 0.0 to + 0.09001 fTempIo + 0.00986 amo + -147.99237;
aso = 0.0 ho + -0.0 lo + -0.0 ro + -0.0 co + 0.0 so + 0.0 to + 0.0 fTempIo + 0.0001 amo + -0.00214;
aTempOo = 0.0 ho + 0.0 lo + -0.31092 ro + 0.07504 co + 0.05491 so + 0.0 to + 0.75181 fTempIo + 0.00032 amo + -0.58188;
fpdo = 0.0 ho + -0.0 lo + 11.36125 ro + -20.763 co + 0.02716 so + 0.0 to + -0.63866 fTempIo + -0.00015 amo + 324.30299;
fso = 0.0 ho + -0.0 lo + 0.00013 ro + -0.07355 co + -2e-05 so + 0.0 to + 0.00051 fTempIo + 0.0 amo + 2.18465;
fTempOo = 0.0 ho + 0.0 lo + 0.3076 ro + -0.07576 co + -0.05474 so + 0.0 to + 0.25176 fTempIo + 0.00028 amo + 12.77172;
po = 0.0 ho + 0.0 lo + 262.73333 ro + 2.74667 co + -26.69657 so + 0.0 to + 0.0 fTempIo + -0.0 amo + 903.0981;

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
ri < 14;
ci > 12;
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
co > 13;
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
