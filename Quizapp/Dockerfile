FROM python:3

WORKDIR /usr/src/app

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

# Create a non-root user
RUN adduser --disabled-password --gecos '' myuser
USER myuser


CMD ["python", "manage.py", "runserver"]
