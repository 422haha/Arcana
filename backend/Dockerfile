# backend/Dockerfile

FROM openjdk:17-jdk-slim

# 작업 디렉토리 설정
WORKDIR /app

# 타임존 설정
RUN ln -sf /usr/share/zoneinfo/Asia/Seoul /etc/localtime

# 애플리케이션 JAR 파일 복사
COPY build/libs/arcana-0.0.1-SNAPSHOT.jar app.jar

# 애플리케이션 실행
ENTRYPOINT ["java", "-jar", "app.jar"]
