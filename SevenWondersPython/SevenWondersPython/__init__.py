"""
The flask application package.
"""

from flask import Flask
app = Flask(__name__)

import SevenWondersPython.views

import pyodbc
cnxn = pyodbc.connect('DRIVER={SQL Server};SERVER=(localdb)\MSSQLLocalDB;DATABASE=NEBRASKA;Trusted_Connection=yes;')
cursor = cnxn.cursor()

cursor.execute("SELECT * FROM dbo.Flights")
for row in cursor.fetchall():
    print row