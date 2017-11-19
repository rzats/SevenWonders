"""
Routes and views for the flask application.
"""

from datetime import datetime
from flask import render_template, current_app
from SevenWondersPython import app

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/home')
def home():
    return render_template('HomePage.html')

@app.route('/contact')
def contact():
    return render_template('Contact.html')

@app.route('/Content/img/<path:path>')
def send_js(path):
    return current_app.send_static_file('content/img/{0}'.format(path))

@app.errorhandler(500)
def error(e):
    return render_template('Error.html'), 500