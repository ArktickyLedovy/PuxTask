# PuxTask
Cvičný úkol pro PUX DESIGN

Assignment (Czech language): 

Napište jednoduchý program, který bude umět detekovat změny v lokálním adresáři
uvedeném na vstupu. Při prvním spuštění si program obsah daného adresáře analyzuje a při
každém dalším spuštění bude hlásit změny od svého posledního spuštění, tj:

a) seznam nových souborů,

b) seznam změněných souborů (změnou se rozumí změna obsahu daného souboru),

c) seznam odstraněných souborů a podadresářů.

U každého souboru evidujte číslo jeho aktuální verze (na začátku budou mít všechny soubory
verzi 1, s každou detekovanou změnou daného souboru bude jeho verze navýšena o 1).
Program realizujte jako jednoduchou ASP.NET WebForms aplikaci naprogramovanou v C#.
Předpokládejte, že velikost souborů v adresáři bude do 50 MB a že počet souborů v každém
adresáři bude nanejvýš 100.
Program se bude spouštět ručně z UI stiskem tlačítka (nedetekujte změny filesystému
automaticky). Nepoužívejte databázi.
UI bude obsahovat alespoň textbox pro zadání cesty k analyzovanému adresáři, tlačítko pro
spuštění analýzy a výpis jejího výsledku.
Své řešení stručně popište a zmiňte i jeho případná omezení.

Příklady:

1/ První spuštění
  Příklad vstupu od uživatele (cesta k adresáři k vyhodnocení):

  ~/data

  Příklad výstupu:

  nový adresář, žádné změny


2/ Opakované spuštění

  Příklad vstupu od uživatele (cesta k adresáři k vyhodnocení):

  ~/data

  Příklad výstupu:

  [A] novysoubor.txt

  [A] podadresar/dalsinovysoubor.docx

  [M] zmeneny.txt (ve verzi 2)

  [D] podadresar/odstraneny.xlsx

  [A] = added (nový soubor)

  [M] = modified (změněný soubor)

  [D] = deleted (odstraněný soubor)


3/ Opakované spuštění

  Příklad vstupu od uživatele (cesta k adresáři k vyhodnocení):

  ~/data

  Příklad výstupu:

  žádná změna
