/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 12.81468 ri + -0.04603 ci + -1.93793 si + 0.0 ti + 0.11061 fTempIi + 0.01803 ami + -154.72056;
asi = 0.0 hi + -0.0 li + -0.00014 ri + -0.00056 ci + -5e-05 si + -0.0 ti + -0.00024 fTempIi + 0.00017 ami + 0.00992;
aTempOi = 0.0 hi + 0.0 li + 0.26131 ri + -0.10684 ci + -0.05336 si + -0.0 ti + 0.80098 fTempIi + -0.00052 ami + 6.77134;
fpdi = 0.0 hi + -0.0 li + 38.84512 ri + -187.94698 ci + -0.08217 si + -0.0 ti + -1.99216 fTempIi + 0.00083 ami + 1070.25045;
fsi = 0.0 hi + -0.0 li + -0.00013 ri + -0.30821 ci + 3e-05 si + -0.0 ti + 0.00064 fTempIi + -0.0 ami + 3.38988;
fTempOi = 0.0 hi + 0.0 li + -0.25802 ri + 0.11 ci + 0.05268 si + 0.0 ti + 0.22333 fTempIi + -0.00049 ami + 10.19984;
pi = 0.0 hi + 0.0 li + 157.175 ri + 3.53333 ci + -16.03429 si + 0.0 ti + 0.0 fTempIi + -0.0 ami + 501.80238;

apdo = 0.0 ho + 0.0 lo + 13.0119 ro + 0.01746 co + -1.3869 so + 0.0 to + 0.0737 fTempIo + 0.01308 amo + -110.9292;
aso = 0.0 ho + -0.0 lo + 0.0 ro + -0.0 co + 0.0 so + -0.0 to + -0.0 fTempIo + 0.00017 amo + -0.00286;
aTempOo = 0.0 ho + -0.0 lo + -0.51642 ro + 0.15248 co + 0.06557 so + 0.0 to + 0.66235 fTempIo + 0.00052 amo + 1.8172;
fpdo = 0.0 ho + 0.0 lo + 9.7414 ro + -19.70381 co + 0.02037 so + 0.0 to + -0.41841 fTempIo + -0.00014 amo + 200.13405;
fso = 0.0 ho + 0.0 lo + 0.00012 ro + -0.10991 co + -1e-05 so + 0.0 to + 0.00029 fTempIo + 0.0 amo + 2.04972;
fTempOo = 0.0 ho + -0.0 lo + 0.5045 ro + -0.14982 co + -0.06372 so + 0.0 to + 0.34875 fTempIo + 0.00042 amo + 11.47629;
po = 0.0 ho + -0.0 lo + 148.19444 ro + 3.50952 co + -7.43095 so + 0.0 to + -0.0 fTempIo + 0.0 amo + 223.08413;

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
ri > 6;
ri < 14;
ci > 1;
ci < 7;
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
ro > 6;
ro < 10;
co > 7;
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
